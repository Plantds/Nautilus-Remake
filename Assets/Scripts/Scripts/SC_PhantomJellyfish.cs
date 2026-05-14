using UnityEngine;

public class SC_PhantomJellyfish : MonoBehaviour
{
    public Rigidbody mainRB;
    public Rigidbody[] rigidbodies;

    public float TimeBetweenJumps = 2.0f;
    private float timer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= TimeBetweenJumps)
        {
            timer = 0.0f;
            mainRB.AddForce(new Vector3(0.0f, 1.0f, 1.0f), ForceMode.VelocityChange);
            // foreach (var rb in rigidbodies)
            // {
            //     Vector2 rand = Random.insideUnitCircle;
            //     rb.AddForce(new Vector3(rand.x, 0.0f, rand.y), ForceMode.Impulse);
            // }
        }

        foreach (var rb in rigidbodies)
        {
            rb.AddForce(new Vector3(0.0f, -0.05f, 0.0f), ForceMode.Impulse);
        }
    }
}
