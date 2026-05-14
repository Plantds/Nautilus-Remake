using UnityEngine;

public class SC_AudioSphere : MonoBehaviour
{
    private Vector3 position;
    private float scale;

    [SerializeField] private FMODUnity.StudioGlobalParameterTrigger trigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = gameObject.transform.position;
        scale = gameObject.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if(!other.CompareTag("Player"))
            return;
        
        Debug.Log(1 - Vector3.Magnitude(position - other.transform.position) / scale);

        trigger.Value = 1 - Vector3.Magnitude(position - other.transform.position) / scale;
        trigger.TriggerParameters();
    }
}
