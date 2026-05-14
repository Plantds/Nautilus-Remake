
public class SC_SubmarineUpLever : SC_LeverObject
{
    public SC_SubmarineMovement submarine;
    public float maxSpeed;
    // void Start()
    // {
        
    // }
    

    void Update ()
    {
        UpdateLever();

        submarine.upInput = progress * maxSpeed;

        if (realDeadZone > progress * leverMaxAngle && submarine.upInput > 0 && realDeadZone > 0 || -realDeadZone < progress * leverMaxAngle && submarine.upInput < 0 && realDeadZone > 0)
        {
            submarine.upInput = 0;
        }
    }
}
