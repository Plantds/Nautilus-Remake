using UnityEngine;

public class SC_FlashlightOffset : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject positionTarget;
    [SerializeField] private float turningResponseSpeed = 3.0f;
    [SerializeField] private float positionResponseSpeed = 3.0f;
    [SerializeField] private float randomResponseSpeed = 3.0f;

    [SerializeField] private float random = 3.0f;

    private Vector3 randomOffset = new Vector3();

    [SerializeField] private LayerMask mask;


    void Start()
    {
        if (!camera)
        {
            Debug.LogWarning("No camera Found for SC_FlashlightOffset");
            return;
        }

        if (!positionTarget)
        {
            Debug.LogWarning("No positionTarget Found for SC_FlashlightOffset");
            return;
        }

        if (turningResponseSpeed <= 0)
        {
            turningResponseSpeed = 1.0f;
        }

        if (positionResponseSpeed <= 0)
        {
            positionResponseSpeed = 1.0f;
        }
    }

    void Update()
    {
        Quaternion lookRotation = new Quaternion();

        RaycastHit info;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        float distance = 10000.0f;
        Vector3 point = camera.transform.position + camera.transform.forward * 10000.0f;

        if (Physics.Raycast(ray, out info, 10000.0f, mask))
        {
            distance = info.distance;
            point = info.point;
        }

        Vector3 randomPosition = new Vector3(Random.Range(-random, random), Random.Range(-random, random), Random.Range(-random, random));

        randomOffset = CustomMath.Lerp.ApproxLerp(randomOffset, randomPosition, randomResponseSpeed, Time.deltaTime);

        Vector3 randomOffsetScaled = randomOffset;

        randomOffsetScaled.x *= distance;
        randomOffsetScaled.y *= distance;
        randomOffsetScaled.z *= distance;

        Vector3 target = randomOffsetScaled + point;

        lookRotation = Quaternion.LookRotation(target - transform.position);

        transform.position = CustomMath.Lerp.ApproxLerp(transform.position, positionTarget.transform.position, positionResponseSpeed, Time.deltaTime);
        transform.rotation = CustomMath.Lerp.ApproxSlerp(transform.rotation, lookRotation, turningResponseSpeed, Time.deltaTime);
    }
}
