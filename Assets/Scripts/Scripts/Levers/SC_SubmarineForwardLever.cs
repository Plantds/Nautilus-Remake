
public class SC_SubmarineForwardLever : SC_LeverObject
{
    public SC_SubmarineMovement submarine;
    public float maxSpeed;
    // void Start()
    // {
        
    // }
    

    void Update()
    {
        UpdateLever();

        submarine.forwardInput = progress * maxSpeed;

        if (realDeadZone > progress * leverMaxAngle && submarine.forwardInput > 0 && realDeadZone > 0 || -realDeadZone < progress * leverMaxAngle && submarine.forwardInput < 0 && realDeadZone > 0)
        {
            submarine.forwardInput = 0;
        }
    }
}
