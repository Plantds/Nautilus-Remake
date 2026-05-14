using UnityEngine;

public abstract class SC_InteractibleObject : MonoBehaviour
{
    public enum DragDirection
    {
        upDown,
        leftRigt
    }
    public DragDirection dragDirection;
    public float dragLenght = 10;
    [Tooltip("When interacting with object goes from -1 and 1. 0 is usaly the starting position\n\nIf you would want the object to start at one end insted of the middle you can set it to 1 or -1")]
    public float progress = 0.0f;
    public bool holding;
    void Start()
    {
        if (dragLenght == 0)
        {
            dragLenght = 1;
        }
    }
}
