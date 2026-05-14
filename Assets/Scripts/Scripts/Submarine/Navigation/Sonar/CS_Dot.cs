using UnityEngine;

public class CS_Dot : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0.0f)
        {
            Destroy(gameObject);
        }
         
    }
}
