using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]
public class PlayerSettingsScriptableObject : ScriptableObject
{
    [Header("Normal Walking Settings")]
    [SerializeField] public float walkSpeed = 10.0f;
    [SerializeField] public float walkResponse = 25.0f;

    [Header("Water Walking Settings")]
    [SerializeField] public float waterWalkSpeed = 5.0f;
    [SerializeField] public float waterWalkResponse = 5.0f;

    [Header("Mouse Settings")]
    [SerializeField] public float mouseSensitivity = 1.0f;

    [Header("Camera Settings")]
    [SerializeField] public float cameraHeightOffset = 0.5f;

}
