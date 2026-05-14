using UnityEngine;

public class SC_LightIntensityLerp : MonoBehaviour
{
    [SerializeField] private Light[] lights;
    private GameObject player;
    [SerializeField] private GameObject objective;

    [Header("Intesity Settings")]
    [SerializeField] private float maxIntesity = 30000;
    [SerializeField] private float minIntesity = 200;
    [SerializeField] private float maxDistance = 80;
    [SerializeField] private float minDistance = 8;


    void Start()
    {
        player = FindAnyObjectByType<CharacterControllerComponent>().gameObject;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, objective.transform.position);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        distance = (distance - minDistance) / (maxDistance - minDistance);

        float intensity = distance * (maxIntesity - minIntesity) + minIntesity;

        foreach (var light in lights)
        {
            light.intensity = intensity;
        }
    }
}
