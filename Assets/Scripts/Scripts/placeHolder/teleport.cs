using KinematicCharacterController;
using UnityEngine;

public class teleport : MonoBehaviour
{
    [SerializeField] GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<KinematicCharacterMotor>().SetPosition(target.transform.position);
        }
    }
}
