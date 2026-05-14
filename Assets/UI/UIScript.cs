using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// FMOD
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    string defaultlevel = "Main Main";
    string loadThisLevel;

    bool pause = false;
    bool menu = true;

    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject optionsButton;
    bool continueGame;
    bool devMenuActive = false;
    bool canSkip = false;
    string levelString;

    public ReloadLoadSave LoadSaveSystem;
    public DevMenu devMenu;
    public Image loadingScreen;


    public VideoPlayer video;
    [SerializeField] GameObject videoPlane;
    int totalSeconds;

    private InputSystem_Actions inputActions;

    // FMOD master bus
    private Bus masterBus;

    public static UIScript Instance { get; private set; }

    public void Awake()
    {
        loadThisLevel = defaultlevel;

        inputActions = new InputSystem_Actions();

        inputActions.Enable();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get FMOD master bus
        masterBus = RuntimeManager.GetBus("bus:/");

    }

    public void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (inputActions.Player.Pause.WasPressedThisFrame() || inputActions.UI.Pause.WasPressedThisFrame())
        {
            OnPause();
        }
        if (Input.anyKey && canSkip)
        {
            videoPlane.SetActive(false);
            video.Stop();
            StartCoroutine(FadeInGame());
        }
    }

    IEnumerator FadeInGame()
    {
        loadingScreen.CrossFadeAlpha(0, 2, false);

        yield return new WaitForSeconds(2);
        loadingScreen.enabled = false;
        yield return null;
    }
    IEnumerator OnVideoStart()
    {
        yield return new WaitForSeconds(1);
        canSkip = true;
    }

    public void OnPause()
    {
        // Stop all FMOD sounds when going back to menu
        StopAllFMODAudio();

        //
        SceneManager.LoadScene("Main Menu");

        mainMenu.SetActive(true);

        inputActions.Player.Disable();
        inputActions.UI.Enable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        //

        // Debug.Log("On Pause Called");

        // if (pause == false)
        // {
        //     pauseMenu.SetActive(true);
        //     optionsButton.SetActive(true);

        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;

        //     inputActions.Player.Disable();
        //     inputActions.UI.Enable();

        //     Time.timeScale = 0;

        //     Debug.Log("Paused");

        //     pause = true;
        //     menu = false;
        // }
        // else if (pause == true)
        // {
        //     pauseMenu.SetActive(false);
        //     optionsButton.SetActive(false);
        //     Time.timeScale = 1;

        //     Cursor.visible = false;
        //     Cursor.lockState = CursorLockMode.Locked;

        //     inputActions.UI.Disable();
        //     inputActions.Player.Enable();

        //     Debug.Log("Unpaused");

        //     pause = false;
        // }
    }

    // FMOD helper: stop all events on the master bus
    private void StopAllFMODAudio()
    {
        if (masterBus.isValid())
        {
            // Use ALLOWFADEOUT for a nicer stop, or IMMEDIATE if you want hard cut
            masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void menuToggle()
    {
        if (menu == true)
        {
            mainMenu.SetActive(true);
        }

        if (menu == false)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void levelSwap(int index)
    {
        switch (index)
        {
            case 0:
                loadThisLevel = "Main Main";
                break;

            case 1:
                loadThisLevel = "L_0_Dressup";
                break;

            case 2:
                loadThisLevel = "L_1_Dressup";
                break;

            case 3:
                loadThisLevel = "L_2_Dressup";
                break;

            case 4:
                loadThisLevel = "GiantsPassBlockout";
                break;
        }
    }

    public void backToMain()
    {
        SceneManager.LoadScene("Main Menu");
        menu = true;
    }

    public void sceneLoader(bool _playVideo)
    {
        // loadThisLevel = "Main Main";
        if (loadThisLevel == "Main Main")
        {
            Debug.Log("Main Was Loaded");

            if (_playVideo)
            {
                double totalTime = video.length;
                totalSeconds = (int)totalTime;

                inputActions.Player.Disable();
                Debug.Log("");
                StartCoroutine(videoPlayer());
            }

        }

        SceneManager.LoadScene(loadThisLevel);

        pause = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;

        // Debug.Log(SceneManager.GetActiveScene().name + " " + loadThisLevel);
        StartCoroutine(whileSceneNotLoaded());

        // if (continueGame)
        // {
        //     Debug.Log("Continue! " + SceneManager.GetActiveScene().name);
        //     LoadSaveSystem.OnContinue();
        // }
        // else
        // {
        //     Debug.Log("NewGame!");
        //     LoadSaveSystem.OnNewGame();
        // }
    }

    IEnumerator whileSceneNotLoaded()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name != loadThisLevel);

        if (continueGame && !devMenuActive)
        {
            LoadSaveSystem.OnContinue();
        }
        else if (!continueGame && !devMenuActive)
        {
            LoadSaveSystem.OnNewGame();
        }
        else if (devMenuActive)
        {
            devMenu.OnDevMenu(levelString);
        }

        yield return null;
    }

    IEnumerator videoPlayer()
    {
        StartCoroutine(OnVideoStart());
        videoPlane.SetActive(true);
        video.Play();

        yield return new WaitForSeconds(totalSeconds);

        loadingScreen.CrossFadeAlpha(0, 2, false);
        inputActions.Player.Enable();
        videoPlane.SetActive(false);
        yield return new WaitForSeconds(2);
        loadingScreen.enabled = false;


        yield return null;
    }

    public void closeGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    public void OnContinue()
    {
        continueGame = true;
        // Debug.Log("Continue!");
        // LoadSaveSystem.OnContinue();
    }
    public void OnNewGame()
    {
        continueGame = false;
        // Debug.Log("NewGame!");
        // LoadSaveSystem.OnNewGame();
    }
    public void OnDevMenu(string lvlString)
    {
        devMenuActive = true;
        levelString = lvlString;
    }
}
