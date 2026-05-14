using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_PlayerFindInteractebleObjects : MonoBehaviour
{
    public InputAction interactInput;
    public InputAction mouseDrag;

    SC_InteractibleObject interactibleObject;
    [SerializeField] Camera cam;
    public float playerReach;
    Ray ray;
    RaycastHit hit;
    bool interact;
    bool holding;
    Vector2 mouseMovementVec;

    void Start()
    {
        interactInput.Enable();
        mouseDrag.Enable();

        interactInput.started += context => interact = true;
        interactInput.started += context => holding = true;
        interactInput.canceled += context => holding = false;
    }


    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * playerReach, Color.red);

        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.gameObject.GetComponent<SC_InteractibleObject>())
            {
                if (interact)
                {
                    onInteract(hit);
                }
            }
        }

        
        if (holding && interactibleObject)
        {
            whileInteract();
            interactibleObject.holding = true;
        }
        
        if (!holding)
        {
            if (interactibleObject)
            {
                interactibleObject.holding = false;
            }

            interactibleObject = null;
        }

        interact = false;
    }

    void onInteract(RaycastHit _hit)
    {
        mouseMovementVec = Vector2.zero;
        interactibleObject = _hit.collider.gameObject.GetComponent<SC_InteractibleObject>();
    }
    
    void whileInteract()
    {   
        mouseMovementVec= Vector2.zero;
        mouseMovementVec += mouseDrag.ReadValue<Vector2>();

        float mouseRelevantMovement = 0;

        if (interactibleObject.dragDirection == SC_InteractibleObject.DragDirection.leftRigt)
        {
            mouseRelevantMovement = mouseMovementVec.x;
        }
        else if (interactibleObject.dragDirection == SC_InteractibleObject.DragDirection.upDown)
        {
            mouseRelevantMovement = mouseMovementVec.y;
        }

        if (mouseRelevantMovement != 0)
        {
            interactibleObject.progress += mouseRelevantMovement / interactibleObject.dragLenght;
            interactibleObject.progress = math.clamp(interactibleObject.progress, -1.0f, 1.0f);
        }
    }
}
