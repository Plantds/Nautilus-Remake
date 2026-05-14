
public class SC_SubmarineTurnLever : SC_LeverObject
{
    public SC_SubmarineMovement submarine;
    public float maxSpeed;
    // void Start()
    // {
        
    // }
    

    void Update ()
    {
        UpdateLever();

        submarine.turnInput = progress * maxSpeed;

        if (realDeadZone > progress * leverMaxAngle && submarine.turnInput > 0 && realDeadZone > 0 || -realDeadZone < progress * leverMaxAngle && submarine.turnInput < 0 && realDeadZone > 0)
        {
            submarine.turnInput = 0;
        }
    }
}
