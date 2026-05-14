using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SharedData
{
    public AnimationCurve curve;

    public Vector3 startPosition;
    public Vector3 endPosition;

    public Vector3 startRotation;
    public Vector3 endRotation;

    public bool SyncWithAnimator;
    public int AnimatorLayerToSync;
    public float time;
}

[Serializable]
public struct ColliderAnimationData
{
    public SharedData data;
}

[Serializable]
public struct ColliderAnimationDataDisplay
{
    public Collider linkedCollider;
    public SharedData data;
}

public class SC_CustomAnimator : MonoBehaviour
{
    [Header("Editor Tools")]
    [Tooltip("Clicking the checkbox will refresh the latest changes if it didn't do it automatically and will reset itself to false")]
    [SerializeField] private bool forceRefresh = false;

    [Header("Automatic Tools")]
    [Tooltip("If true, then it will automatically find the colliders in this gameobject")]
    [SerializeField] private bool autoFindColliders = false;
    [Tooltip("If true, then it will automatically find the colliders in childed gameobjects")]
    [SerializeField] private bool autoFindChildColliders = false;

    [Header("Drag Colliders Here")]
    [SerializeField] private List<Collider> colliders = new List<Collider>();

    [Header("Dangerous Tools")]
    [Tooltip("Clicking the checkbox will reset the Animated Colliders back to default")]
    public bool forceHardReset = false;

    [Header("Animations")]
    [SerializeField] private List<ColliderAnimationDataDisplay> displayReflection = new List<ColliderAnimationDataDisplay>();

    [SerializeField] private List<string> animationsToPlay = new List<string>();

    private Dictionary<Collider, ColliderAnimationData> dictionary = new Dictionary<Collider, ColliderAnimationData>();

    private List<Collider> removalCommandBuffer = new List<Collider>();

    private Animator anim;

    float time = 0.0f;

    void OnValidate()
    {
        forceRefresh = false;

        if (forceHardReset)
        {
            forceHardReset = false;
            dictionary.Clear();
            displayReflection.Clear();
        }

        RemoveData();
        RefreshData();
        ReflectData();
    }

    void RemoveData()
    {
        removalCommandBuffer = new List<Collider>();

        foreach (var itr in dictionary)
        {
            if (!colliders.Contains(itr.Key))
            {
                removalCommandBuffer.Add(itr.Key);
            }
        }

        foreach (var cmd in removalCommandBuffer)
        {
            dictionary.Remove(cmd);
        }

        removalCommandBuffer.Clear();
    }

    void RefreshData()
    {
        foreach (var collider in colliders)
        {
            if (dictionary.ContainsKey(collider))
                continue;

            ColliderAnimationData animData = new ColliderAnimationData();
            SharedData sharedData = new SharedData();

            //Curve
            sharedData.curve = new AnimationCurve();
            sharedData.curve.AddKey(0.0f, 0.0f);
            sharedData.curve.AddKey(1.0f, 1.0f);

            //Positions
            sharedData.startPosition = new Vector3();
            sharedData.endPosition = new Vector3();

            sharedData.SyncWithAnimator = false;

            sharedData.AnimatorLayerToSync = 0;

            sharedData.time = 0.0f;

            sharedData.startRotation = new Vector3();
            sharedData.endRotation = new Vector3();

            animData.data = sharedData;

            dictionary.Add(collider, animData);
        }
    }

    void ReflectData()
    {
        foreach (var itr in displayReflection)
        {
            if (!dictionary.ContainsKey(itr.linkedCollider))
                continue;

            ColliderAnimationData animationData = new ColliderAnimationData();
            SharedData sharedData = new SharedData();

            sharedData.curve = itr.data.curve;
            sharedData.startPosition = itr.data.startPosition;
            sharedData.endPosition = itr.data.endPosition;
            sharedData.SyncWithAnimator = itr.data.SyncWithAnimator;
            sharedData.AnimatorLayerToSync = itr.data.AnimatorLayerToSync;
            sharedData.time = itr.data.time;

            sharedData.startRotation = itr.data.startRotation;
            sharedData.endRotation = itr.data.endRotation;

            animationData.data = sharedData;

            dictionary[itr.linkedCollider] = animationData;
        }

        displayReflection.Clear();
        foreach (var itr in dictionary)
        {
            ColliderAnimationDataDisplay reflectionData = new ColliderAnimationDataDisplay();

            reflectionData.linkedCollider = itr.Key;
            reflectionData.data.curve = itr.Value.data.curve;
            reflectionData.data.startPosition = itr.Value.data.startPosition;
            reflectionData.data.endPosition = itr.Value.data.endPosition;
            reflectionData.data.SyncWithAnimator = itr.Value.data.SyncWithAnimator;
            reflectionData.data.AnimatorLayerToSync = itr.Value.data.AnimatorLayerToSync;

            reflectionData.data.startRotation = itr.Value.data.startRotation;
            reflectionData.data.endRotation = itr.Value.data.endRotation;

            displayReflection.Add(reflectionData);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();

        foreach (var name in animationsToPlay)
        {
            anim.Play(name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        foreach (var key in colliders)
        {
            var value = dictionary[key];

            if (!value.data.SyncWithAnimator)
                continue;

            value.data.time += delta;
            dictionary[key] = value;

            float time = value.data.time;
            float normalizedTime = Mathf.Clamp(time / anim.GetCurrentAnimatorStateInfo(value.data.AnimatorLayerToSync).length, 0.0f, 1.0f);

            anim.SetFloat("Time", normalizedTime);

            key.transform.localPosition = Vector3.LerpUnclamped(value.data.startPosition, value.data.endPosition, value.data.curve.Evaluate(normalizedTime));
            //key.transform.
        }


    }
}