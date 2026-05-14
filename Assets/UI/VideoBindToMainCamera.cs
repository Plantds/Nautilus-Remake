using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoBindToMainCamera : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void OnEnable()
    {
        // Bind immediately
        BindCamera();

        // Re-bind every time a new scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindCamera();
    }

    private void BindCamera()
    {
        if (videoPlayer == null)
            return;

        
        if (videoPlayer.renderMode == VideoRenderMode.CameraFarPlane ||
            videoPlayer.renderMode == VideoRenderMode.CameraNearPlane)
        {
            if (Camera.main != null)
            {
                videoPlayer.targetCamera = Camera.main;
            }
        }
    }
}
