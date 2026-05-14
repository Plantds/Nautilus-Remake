using UnityEngine;

public class AirlockStateEmissionIndicator : MonoBehaviour
{
    [Header("Airlock References (for programmer to use)")]
    [SerializeField] private SubmarineAirlock airlock;
    [SerializeField] private SC_SubmarineAirlockSequence airlockSequence;

    [Header("State Objects")]
    [SerializeField] private GameObject insideObject;
    [SerializeField] private GameObject outsideObject;
    [SerializeField] private GameObject inBetweenObject;

    [Header("In-Between Emission Blink")]
    [SerializeField] private Material inBetweenMaterial;
    [SerializeField] private string emissiveStrengthProperty = "_Emissive_Strength";
    [SerializeField] private float minEmission = 0f;
    [SerializeField] private float maxEmission = 300f;
    [SerializeField] private float blinkSpeed = 2f;

    private float originalEmission;
    private bool hasOriginalEmission;

    private void Awake()
    {
        if (inBetweenMaterial != null && inBetweenMaterial.HasProperty(emissiveStrengthProperty))
        {
            originalEmission = inBetweenMaterial.GetFloat(emissiveStrengthProperty);
            hasOriginalEmission = true;
        }
    }

    private void OnDisable()
    {
        RestoreEmission();
    }

    private void Update()
    {
        if (airlock == null || airlockSequence == null)
            return;

        bool inBetween = IsInBetween();
        bool isInside  = IsInside();
        bool isOutside = IsOutside();

        if (inBetween)
        {
            SetActive(insideObject, false);
            SetActive(outsideObject, false);
            SetActive(inBetweenObject, true);
            BlinkEmission();
        }
        else
        {
            RestoreEmission();
            SetActive(inBetweenObject, false);
            SetActive(insideObject, isInside);
            SetActive(outsideObject, isOutside);
        }
    }

    // HELP

    private bool IsInBetween()
    {
        // TODO: return true while airlock is transitioning
        // e.g. check airlockSequence / animation players
        return false;
    }

    private bool IsInside()
    {
        // TODO: return true when "safe inside" state is reached
        return false;
    }

    private bool IsOutside()
    {
        // TODO: return true when "outside" state is reached
        return false;
    }

    // ----- EMISSION + HELPERS -----

    private void BlinkEmission()
    {
        if (inBetweenMaterial == null || !inBetweenMaterial.HasProperty(emissiveStrengthProperty))
            return;

        float t = 0.5f * (Mathf.Sin(Time.time * blinkSpeed * Mathf.PI * 2f) + 1f);
        float value = Mathf.Lerp(minEmission, maxEmission, t);
        inBetweenMaterial.SetFloat(emissiveStrengthProperty, value);
    }

    private void RestoreEmission()
    {
        if (!hasOriginalEmission || inBetweenMaterial == null)
            return;

        if (!inBetweenMaterial.HasProperty(emissiveStrengthProperty))
            return;

        inBetweenMaterial.SetFloat(emissiveStrengthProperty, originalEmission);
    }

    private void SetActive(GameObject go, bool active)
    {
        if (go != null && go.activeSelf != active)
            go.SetActive(active);
    }
}
