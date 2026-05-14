using UnityEngine;

public class SC_LeverObject : SC_InteractibleObject
{
    public GameObject pivot;

    [Tooltip("Deadzone in degrees for when the lever is set to 0\n\nIf leverMaxAngle is set to 90, then 180 makes it so every possible angle sets the lever to 0")]
    public float deadZone;
    [HideInInspector]
    public float realDeadZone;
    [Tooltip("The max angle a lever can go in one direction\n\nIf 90 then it can go 90 degrees front and 90 degrees back direction")]
    public float leverMaxAngle;
    public bool flipPositiveAngle;
    int flipAngleInt;
    Vector3 startEulerAngles;

    public enum RotationAxis
    {
        x,
        y,
        z,
    }
    public RotationAxis rotationAxis;

    //For Unfinished snaping logic
    // public bool snapToEnd;
    // public float snapSpeed;
    // float snapTimer;
    void Start()
    {
        if (deadZone != 0)
        {
            realDeadZone = deadZone / 2;
        }

        if (flipPositiveAngle)
        {
            flipAngleInt = -1;
        }
        else
        {
            flipAngleInt = 1;
        }

        startEulerAngles = pivot.transform.localEulerAngles;
    }
    

    public void UpdateLever()
    {
        //Unfinished snaping logic
        // if (snapToEnd && progress * leverMaxAngle > realDeadZone && !holding)
        // {
        //     snapTimer += Time.deltaTime;
        //     if (progress + snapTimer/snapSpeed > 1)
        //     {
        //         snapTimer = 0;
        //     }
        //     progress = math.lerp(0,1, progress + snapTimer/snapSpeed);
        // }

        float x = startEulerAngles.x;
        float y = startEulerAngles.y;
        float z = startEulerAngles.z;
        
        switch (rotationAxis)
        {
            case RotationAxis.x:
                x += progress * leverMaxAngle * flipAngleInt;
                break;
            case RotationAxis.y:
                y += progress * leverMaxAngle * flipAngleInt;
                break;
            case RotationAxis.z:
                z += progress * leverMaxAngle * flipAngleInt;
                break;
        }

        pivot.transform.localEulerAngles = new Vector3(x, y, z);

        //Make so deadzone logic is universial with all levers
    }
}
