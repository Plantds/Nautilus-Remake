using UnityEngine;

public class CS_Floating : MonoBehaviour
{
    public float FloatStrenght;
    public float RandomRotationStrenght;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    void Update()
    {
        rb.AddForce(Vector3.up * FloatStrenght);
        transform.Rotate(RandomRotationStrenght, RandomRotationStrenght, RandomRotationStrenght);
    }
}
