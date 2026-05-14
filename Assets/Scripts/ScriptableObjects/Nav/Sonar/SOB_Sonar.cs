using UnityEngine;

[CreateAssetMenu(fileName = "SOB_Sonar", menuName = "Scriptable Objects/Nav/SOB_Sonar")]
public class SOB_Sonar : ScriptableObject
{
    [Header("Rays")]
    [Tooltip("Length of the rays, how far they will travel")]
    [Range(0.0f, 10000.0f)]
    public float lengthOfRay = 1000.0f;

    [Header("Horizontal")]
    [Tooltip("Amount of rays the a full lap of the sonar")]
    [Range(3, 360)]
    public int amountOfRaysInHorizontal = 10;
    [Tooltip("How long a full lap of the sonar will take")]
    [Min(0.001f)]
    public float lapTime = 10.0f;

    [Header("Vertical")]
    [Tooltip("Amount of rays that will be fired everytime the sonar makes a check")]
    [Range(0, 180)]
    public int amountOfRaysInVertical = 10;
    [Tooltip("The max degree the vertical rays will be shot at")]
    [Range(0.0f, 180.0f)]
    public float maxDegreeInVertical = 45.0f;

    [Header("LayerMask")]
    [Tooltip("The layer(s) that the sonar will look for")]
    public LayerMask hitLayer = 0;

    [Header("on/off")]
    [Tooltip("if enabled then the sonar will run else it will not")]
    public bool _enabled = true;

    [Header("Debug")]
    [Tooltip("True = seeing hit rays")]
    public bool hitDebug = false;
    [Tooltip("True = seeing miss rays")]
    public bool missDebug = false;
    [Min(0.001f)]
    [Tooltip("How long the miss rays will stay")]
    public float missDebugLifeTime = 1.0f;
    [Min(0.001f)]
    [Tooltip("How long the hit rays will stay")]
    public float hitDebugLifeTime = 1.0f;
}
