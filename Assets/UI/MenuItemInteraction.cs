using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using FMODUnity;   // For EventReference + RuntimeManager

public class MenuItemInteraction : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public Image gearImage;              // UI Image with gear sprite
    public TextMeshProUGUI label;        // Menu label text

    [Header("Text Colors")]
    [Tooltip("Dim color when not selected/hovered")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.4f);   // dim
    [Tooltip("Bright color on hover")]
    public Color hoverColor = new Color(1f, 1f, 1f, 1f);      // bright

    [Header("Gear Fade")]
    [Range(0f, 1f)] public float gearVisibleAlpha = 1f;
    [Range(0f, 1f)] public float gearHiddenAlpha = 0f;
    public float gearFadeSpeed = 10f;

    [Header("Gear Rotation")]
    public bool useRotation = true;
    [Tooltip("Degrees per second while hovered")]
    public float hoverRotationSpeed = 90f;

    [Header("Scale (extra juice)")]
    public bool useScale = true;
    public float hoverScale = 1.05f;
    public float pressedScale = 0.95f;
    public float scaleLerpSpeed = 10f;

    [Header("Slide (move button on hover)")]
    public bool useSlide = false;
    [Tooltip("Offset from original anchored position while hovered (x,y)")]
    public Vector2 hoverOffset = new Vector2(20f, 0f);
    [Tooltip("How fast the slide blend (0–1) moves towards target")]
    public float slideLerpSpeed = 10f;

    [Header("FMOD")]
    [Tooltip("FMOD event to play when the pointer hovers this item.")]
    public EventReference hoverEvent;
    [Tooltip("FMOD event to play when this item is pressed / clicked.")]
    public EventReference clickEvent;

    // ----- internals -----
    Vector3 originalScale;
    Vector3 targetScale;

    Color gearBaseColor;
    float gearTargetAlpha;

    bool isHovered = false;

    RectTransform rectTransform;
    Vector2 originalAnchoredPos;

    // blend: 0 = no offset, 1 = full hoverOffset
    float slideBlend = 0f;
    float slideBlendTarget = 0f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Text starts dim
        if (label != null)
            label.color = normalColor;

        // Gear starts invisible (alpha 0) and pivot centered
        if (gearImage != null)
        {
            // Force pivot to center so rotation is from the middle
            RectTransform rt = gearImage.rectTransform;
            rt.pivot = new Vector2(0.5f, 0.5f);

            gearBaseColor = gearImage.color;
            gearTargetAlpha = gearHiddenAlpha;

            Color startColor = gearBaseColor;
            startColor.a = gearHiddenAlpha;
            gearImage.color = startColor;
        }

        originalScale = rectTransform.localScale;
        targetScale = originalScale;
    }

    void Start()
    {
        // Capture anchored position AFTER layout is done (important for builds/layout groups)
        if (rectTransform != null)
        {
            originalAnchoredPos = rectTransform.anchoredPosition;
        }
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        // Smooth scale
        if (useScale)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                dt * scaleLerpSpeed
            );
        }

        // Smooth slide using blend 0–1, not from "current"
        if (useSlide && rectTransform != null)
        {
            slideBlend = Mathf.MoveTowards(slideBlend, slideBlendTarget, slideLerpSpeed * dt);

            Vector2 hoverPos = originalAnchoredPos + hoverOffset;
            rectTransform.anchoredPosition = Vector2.Lerp(
                originalAnchoredPos,
                hoverPos,
                slideBlend
            );
        }

        // Smooth gear fade
        if (gearImage != null)
        {
            Color current = gearImage.color;
            float newAlpha = Mathf.Lerp(
                current.a,
                gearTargetAlpha,
                dt * gearFadeSpeed
            );

            gearImage.color = new Color(
                gearBaseColor.r,
                gearBaseColor.g,
                gearBaseColor.b,
                newAlpha
            );
        }

        // Gear rotation on hover
        if (useRotation && isHovered && gearImage != null)
        {
            gearImage.rectTransform.Rotate(
                0f,
                0f,
                -hoverRotationSpeed * dt
            );
        }
    }

    // ----- Pointer events -----

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        gearTargetAlpha = gearVisibleAlpha;

        if (label != null)
            label.color = hoverColor;

        if (useScale)
            targetScale = originalScale * hoverScale;

        if (useSlide)
            slideBlendTarget = 1f;

        PlayHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        gearTargetAlpha = gearHiddenAlpha;

        if (label != null)
            label.color = normalColor;

        if (useScale)
            targetScale = originalScale;

        if (useSlide)
            slideBlendTarget = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (useScale)
            targetScale = originalScale * pressedScale;

        PlayClickSound();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (useScale)
            targetScale = originalScale * hoverScale;
    }

    // ----- FMOD helpers -----

    void PlayHoverSound()
    {
        if (!hoverEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(hoverEvent, Vector3.zero);
        }
    }

    void PlayClickSound()
    {
        if (!clickEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(clickEvent, Vector3.zero);
        }
    }
}
