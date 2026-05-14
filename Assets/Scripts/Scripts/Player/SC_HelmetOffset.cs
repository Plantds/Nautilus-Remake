using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SC_HelmetOffset : MonoBehaviour
{
    [SerializeField] private GameObject positionTarget;
    //[SerializeField] private float turningResponseSpeed = 3.0f;

    void Start()
    {
        if (!positionTarget)
        {
            Debug.LogWarning("No positionTarget Found for SC_FlashlightOffset");
            return;
        }
    }

    public void ManualUpdate(Vector3 target, Quaternion rotation)
    {
        transform.position = target;
        transform.rotation = rotation; //CustomMath.Lerp.ApproxSlerp(transform.rotation, rotation, turningResponseSpeed, Time.deltaTime);
    }
}
