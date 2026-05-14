using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_DeapthMeter : CS_ElctricalBaseScript
{
    [Header("Depth Raycast Points")]
    [SerializeField] private GameObject topLeft;
    [SerializeField] private GameObject bottomLeft;
    [SerializeField] private GameObject topRight;
    [SerializeField] private GameObject bottomRight;

    [Header("Raycast Settings")]
    [SerializeField] private float rayLength = 10.0f;
    [SerializeField] private LayerMask hitlayer = 0;

    [Header("Gauge (Needle) Settings")]
    [SerializeField] private Transform gaugeNeedle;
    [SerializeField] private float minAngle = -90f; // angle at distance 0
    [SerializeField] private float maxAngle = 90f;  // angle at rayLength
    [SerializeField] private bool rotateX = false;
    [SerializeField] private bool rotateY = false;
    [SerializeField] private bool rotateZ = true;

    [SerializeField] private bool smoothGauge = true;
    [SerializeField] private float gaugeSpeed = 8f;


    private float Ltimer = 0.0f;
    private RaycastHit hitray = new RaycastHit();
    private List<Ray> rays = new List<Ray>();
    private SortedDictionary<float, Vector3> hits = new SortedDictionary<float, Vector3>();
    private bool _active = true;

    // 0 = at origin of ray, 1 = at rayLength or max depth <3
    private float normalizedDepth = 0f;
    private float currentGaugeAngle = 0f;

    private void getPos()
    {
        rays.Clear();

        rays.Add(new Ray(topLeft.transform.position, -transform.up));
        rays.Add(new Ray(bottomLeft.transform.position, -transform.up));
        rays.Add(new Ray(topRight.transform.position, -transform.up));
        rays.Add(new Ray(bottomRight.transform.position, -transform.up));
    }

    private void getHit()
    {
        hits.Clear();
        hitray = new RaycastHit();

        foreach (Ray r in rays)
        {
            if (Physics.Raycast(r.origin, r.direction, out hitray, rayLength, hitlayer))
            {
                Debug.DrawRay(r.origin, r.direction * rayLength, Color.green, 1.5f);
                hits.TryAdd(hitray.distance, hitray.point);
            }
            else
            {
                Debug.DrawRay(r.origin, r.direction * rayLength, Color.red, 1.0f);
            }
        }
    }

    private void DistanceCheck()
    {
        float sd = hits.First().Key; // shortest distance
        normalizedDepth = Mathf.Clamp01(sd / rayLength); // 0..1
    }

    private void UpdateGauge()
    {
        if (_active)
        {
            if (gaugeNeedle == null)
                return;

            // Map 0..1 depth to angle
            float targetAngle = Mathf.Lerp(minAngle, maxAngle, normalizedDepth);

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

            // Map 0..1 depth to angle
            float targetAngle = Mathf.Lerp(minAngle, maxAngle, 0.0f);

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
        Ltimer += Time.deltaTime;

        if (Ltimer >= 1.0f)
        {
            getPos();
            getHit();

            if (hits.Count > 0)
            {
                DistanceCheck();
            }
            else
            {
                // No hit = treat as max depth
                normalizedDepth = 1f;
            }

            Ltimer = 0.0f;
        }

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
