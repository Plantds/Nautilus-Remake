using UnityEngine;

public class SC_Floating : MonoBehaviour
{
    public float FloatStrenght;
    public float RandomRotationStrenght;
    private Rigidbody rb;

    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        rb.AddForce(Vector3.up * -FloatStrenght);
        //transform.Rotate(RandomRotationStrenght, RandomRotationStrenght, RandomRotationStrenght);
    }
}
