using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// FMOD
using FMODUnity;
using FMOD.Studio;

public class SettingsMenu : MonoBehaviour
{
    [Header("Panel")]
    public GameObject settingsPanel;

    [Header("Video")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Audio")]
    public Slider volumeSlider;          // Controls FMOD master bus

    [Header("Gameplay")]
    public Slider sensitivitySlider;

    [Header("Optional")]
    public AudioSource clickSound;

    private Resolution[] resolutions;

    // FMOD master bus
    //
    private const string MASTER_BUS_PATH = "bus:/";
    private Bus masterBus;

    void Awake()
    {
        // Get reference to FMOD master bus
        masterBus = RuntimeManager.GetBus(MASTER_BUS_PATH);
    }

    void Start()
    {
        SetupResolutions();
        LoadSettings();
    }

    // -------------------- OPEN / CLOSE --------------------

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // -------------------- INIT --------------------

    void SetupResolutions()
    {
        if (resolutionDropdown == null)
            return;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution res = resolutions[i];
            string option = res.width + " x " + res.height;
            options.Add(option);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedIndex = PlayerPrefs.GetInt("Resolution", currentIndex);
        savedIndex = Mathf.Clamp(savedIndex, 0, resolutions.Length - 1);

        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        ApplyResolution(savedIndex);
    }

    void LoadSettings()
    {
        // Fullscreen
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = fullscreen;
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = fullscreen;

        // FMOD Volume
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        if (volumeSlider != null)
            volumeSlider.value = volume;
        SetFMODVolume(volume);

        // Sensitivity
        float sens = PlayerPrefs.GetFloat("MouseSensitivity", 3f);
        if (sensitivitySlider != null)
            sensitivitySlider.value = sens;
    }

    void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    // -------------------- UI CALLBACKS --------------------

    public void OnResolutionChange()
    {
        if (resolutionDropdown == null || resolutions == null || resolutions.Length == 0)
            return;

        int index = Mathf.Clamp(resolutionDropdown.value, 0, resolutions.Length - 1);
        ApplyResolution(index);

        PlayerPrefs.SetInt("Resolution", index);
        SaveSettings();
        PlayClick();
    }

    void ApplyResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void OnFullscreenChange()
    {
        if (fullscreenToggle == null)
            return;

        bool isFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        SaveSettings();
        PlayClick();
    }

    // FMOD MASTER VOLUME
    public void OnVolumeChange()
    {
        if (volumeSlider == null)
            return;

        float v = volumeSlider.value;

        SetFMODVolume(v);

        PlayerPrefs.SetFloat("MasterVolume", v);
        SaveSettings();
        PlayClick();
    }

    void SetFMODVolume(float v)
    {
        if (masterBus.isValid())
        {
            masterBus.setVolume(v); // 0.0–1.0
        }
    }

    public void OnSensitivityChange()
    {
        if (sensitivitySlider == null)
            return;

        float s = sensitivitySlider.value;
        PlayerPrefs.SetFloat("MouseSensitivity", s);
        SaveSettings();
        PlayClick();
    }

    void PlayClick()
    {
        if (clickSound != null && clickSound.clip != null)
            clickSound.PlayOneShot(clickSound.clip);
    }
}
