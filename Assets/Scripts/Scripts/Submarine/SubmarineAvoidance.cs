using UnityEngine;
using UnityEngine.VFX;

public class SubmarineAvoidance : MonoBehaviour
{
    public VisualEffect m_VFX;

    private static readonly int kSharkPositionID = Shader.PropertyToID("Shark_Position");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_VFX.SetVector3(kSharkPositionID, transform.position);
    }
}
