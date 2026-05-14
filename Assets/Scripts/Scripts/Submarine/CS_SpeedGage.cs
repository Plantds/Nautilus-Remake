using System.Data.Common;
using UnityEngine;

public class CS_SpeedGage : CS_ElctricalBaseScript
{
    [SerializeField] private SC_SubmarineMovement sm;

    [Header("Gauge (Needle) Settings")]
    [SerializeField] private Transform gaugeNeedle;
    [SerializeField] private float minAngle = -90f; // angle at distance 0
    [SerializeField] private float maxAngle = 90f;  // angle at rayLength
    [SerializeField] private bool rotateX = false;
    [SerializeField] private bool rotateY = false;
    [SerializeField] private bool rotateZ = true;

    [SerializeField] private bool smoothGauge = true;
    [SerializeField] private float gaugeSpeed = 8f;
    
    private float velocityMagnitude = 0f;
    private float currentGaugeAngle = 0f;
    private bool _active = true;

    private float GetSubSpeed()
    {
        return sm.LinearVelocity.magnitude;
    }

    private void UpdateGauge()
    {
        if (_active)
        {
            if (gaugeNeedle == null)
                return;

            // 0 -> 1. 1 is the max speed the sub can go
            velocityMagnitude /= sm.forwardMaxSpeed;

            float targetAngle = Mathf.Lerp(minAngle, maxAngle, velocityMagnitude);
            //Debug.Log("velm "+ velocityMagnitude + " target " + targetAngle);

            if (smoothGauge)
            {
                currentGaugeAngle = Mathf.Lerp(currentGaugeAngle, targetAngle, Time.deltaTime * gaugeSpeed);
            }
            else
            {
                currentGaugeAngle = targetAngle;
            }

            Vector3 euler = gaugeNeedle.localEulerAngles;

            if (rotateX) euler.x = currentGaugeAngle;
            if (rotateY) euler.y = currentGaugeAngle;
            if (rotateZ) euler.z = currentGaugeAngle;

            gaugeNeedle.localEulerAngles = euler;
        }
        else
        {
            if (gaugeNeedle == null)
                return;

            float targetAngle = Mathf.Lerp(minAngle, maxAngle, 0.0f);
            //Debug.Log("velm "+ velocityMagnitude + " target " + targetAngle);

            if (smoothGauge)
            {
                currentGaugeAngle = Mathf.Lerp(currentGaugeAngle, targetAngle, Time.deltaTime * (gaugeSpeed/20.0f));
            }
            else
            {
                currentGaugeAngle = targetAngle;
            }

            Vector3 euler = gaugeNeedle.localEulerAngles;

            if (rotateX) euler.x = currentGaugeAngle;
            if (rotateY) euler.y = currentGaugeAngle;
            if (rotateZ) euler.z = currentGaugeAngle;

            gaugeNeedle.localEulerAngles = euler;
        }
    }

    private void Update()
    {
        velocityMagnitude = GetSubSpeed();
        UpdateGauge();
    }

    public override void TurnOn(bool flash)
    {
        _active = true;
    }

    public override void TurnOff(bool flash)
    {
        _active = false;
    }

}
