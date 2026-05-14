using System.Collections;
using AASave;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.UI;

public class ReloadLoadSave : MonoBehaviour
{
    public enum deathType
    {
        player,
        submarine
    }
    SaveSystem saveSystem;
    LevelStreamingManager levelManager;
    CharacterControllerComponent player;
    SC_SubmarineMovement sub;
    SubmarineDamageSystem damageSystem;

    SubmarineAirlock airlock;
    [SerializeField] FadeOutImg fadeOut;
    Image fadeOutImg;
    [SerializeField] float fadeInTime = 1;
    [SerializeField] float fadeOutTime = 1;
    bool loaded = false;
    GameManager gameManager;
    [SerializeField] public SC_SubmarineAirlockSequence airlockSequence;
    [SerializeField] private UnityEngine.UI.Image uiImage;


    void Start()
    {
        // FindObjects();
    }

    public void OnNewGame()
    {
        StartCoroutine(LoadingNewGame());
    }
    public void OnContinue()
    {
        FindObjects();
        if (saveSystem.DoesDataExists("CurrentLvl"))
        {
            LoadLvl();
        }
        else
        {
            StartCoroutine(LoadingNewGame());
        }
    }
    IEnumerator LoadingNewGame()
    {
        FindObjects();
        saveSystem.Delete("CurrentLvl");
        saveSystem.Delete("SubPos");
        saveSystem.Delete("SubRot");
        saveSystem.Delete("PlayerPos");
        saveSystem.Delete("PlayerRot");
        saveSystem.Delete("TriggerIds");
        saveSystem.Delete("TriggerTempIds");
        saveSystem.Delete("SubHp");
        saveSystem.Delete("AirlockOpen");
        loaded = false;

        FindObjects();

        StartCoroutine(levelManager.LoadScene("Main"));
        StartCoroutine(levelManager.LoadScene(levelManager.trainCartScenesNames[0]));
        StartCoroutine(levelManager.LoadScene("Doors"));
        // int currentSceneIndex = levelManager.trainCartNameToIndexMap[currentScene];
        //Only load current lvl?? 

        //If the OneTimeTriggers dont get turned of correctly this is the problem!!
        yield return new WaitUntil(() => levelManager.leavelLoadingAmount <= 0);
        FindObjects();

        gameManager.SetPlayerState(gameManager.currentPlayerState);

        loaded = true;
        yield return null;
    }


    public void FindObjects()
    {
        saveSystem = FindAnyObjectByType<SaveSystem>();
        if (saveSystem == null)
            Debug.Log("PlayerSubDeath_Error: No SaveSystem found");

        levelManager = FindAnyObjectByType<LevelStreamingManager>();
        if (levelManager == null)
            Debug.Log("PlayerSubDeath_Error: No LevelStreamingManager found");

        player = FindAnyObjectByType<CharacterControllerComponent>();
        if (player == null)
            Debug.Log("PlayerSubDeath_Error: No Player found");

        sub = FindAnyObjectByType<SC_SubmarineMovement>();
        if (sub == null)
            Debug.Log("PlayerSubDeath_Error: No Submarine found");

        damageSystem = FindAnyObjectByType<SubmarineDamageSystem>();
        if (damageSystem == null)
            Debug.Log("PlayerSubDeath_Error: No SubmarineDamageSystem found");

        airlock = FindAnyObjectByType<SubmarineAirlock>();
        if (airlock == null)
            Debug.Log("PlayerSubDeath_Error: No Airlock found");
        fadeOut = FindAnyObjectByType<FadeOutImg>();
        if (fadeOut == null)
            Debug.Log("PlayerSubDeath_Error: No FadeOutImg found");
        else
        {
            fadeOutImg = fadeOut.GetComponent<Image>();
            if (!fadeOutImg.enabled)
            {
                fadeOutImg.enabled = true;
                fadeOutImg.CrossFadeAlpha(0.0f, 0.0f, false);
            }
        }

        airlockSequence = FindAnyObjectByType<SC_SubmarineAirlockSequence>();
        if (airlockSequence == null)
            Debug.Log("PlayerSubDeath_Error: No submarineAirlockSequence found");

        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
            Debug.Log("PlayerSubDeath_Error: No gameManager found");
        
    }
    public void Death(deathType _deathType)
    {
        StartCoroutine(DeathIEnumerator(_deathType));
    }

    IEnumerator DeathIEnumerator(deathType _deathType)
    {
        //Respawn
        fadeOutImg.CrossFadeAlpha(1, fadeInTime, false);
        yield return new WaitForSeconds(fadeInTime);

        StartCoroutine(ReloadLvl());

        if (_deathType == deathType.player)
        {
            PlayerRespawn();
        }
        else if (_deathType == deathType.submarine)
        {
            SubmarineRespawn();
        }

        yield return new WaitUntil(() => loaded);
        fadeOutImg.CrossFadeAlpha(0, fadeOutTime, false);
        // yield return new WaitForSeconds(fadeOutTime);

        yield return null;
    }

    IEnumerator ReloadLvl()
    {
        loaded = false;

        FindObjects();
        string currentScene = saveSystem.Load("CurrentLvl");

        StartCoroutine(levelManager.UnloadScene(currentScene));
        StartCoroutine(levelManager.UnloadScene("Doors"));
        StartCoroutine(levelManager.UnloadScene("Main"));

        // yield return new WaitUntil(() => levelManager.mainReloadingAmount <= 0);
        yield return new WaitUntil(() => levelManager.leavelUnloadingAmount <= 0);

        StartCoroutine(levelManager.LoadScene("Main"));
        StartCoroutine(levelManager.LoadScene("Doors"));
        StartCoroutine(levelManager.LoadScene(currentScene));


        //If the OneTimeTriggers dont get turned of correctly this is the problem!!
        yield return new WaitUntil(() => levelManager.leavelLoadingAmount <= 0);
        yield return new WaitUntil(() => FindObjectsByType<OneTimeTriggers>(FindObjectsSortMode.None) != null);
        FindObjects();

        //Disable img 
        uiImage.gameObject.SetActive(false);


        OneTimeTriggers[] eventTriggers = FindObjectsByType<OneTimeTriggers>(FindObjectsSortMode.None);
        string[] ids = saveSystem.LoadArray("TriggerIds").AsStringArray();

        //Wanted to use maps but this saving system plugin didnt support any type of maps
        foreach (var trigger in eventTriggers)
        {
            foreach (var id in ids)
            {
                if (trigger.id == id)
                {
                    trigger.GetComponent<Collider>().enabled = false;
                }
            }
        }


        //Also save camera rotation
        sub.SetPositionAndRotation(saveSystem.Load("SubPos").AsVector3(), saveSystem.Load("SubRot").AsQuaternion());
        player.GetComponent<KinematicCharacterMotor>().SetPositionAndRotation(saveSystem.Load("PlayerPos").AsVector3(), saveSystem.Load("PlayerRot").AsQuaternion());

        PlayerState _state = PlayerState.OUTSIDE_SUBMARINE;
        if (!saveSystem.Load("AirlockOpen").AsBool())
        {
            _state = PlayerState.INSIDE_SUBMARINE;
        }
        gameManager.SetPlayerState(_state);

        loaded = true;
        yield return null;
    }

    public void LoadLvl()
    {
        StartCoroutine(LoadingLevel());
        
    }
    IEnumerator LoadingLevel()
    {
        loaded = false;
        FindObjects();

        StartCoroutine(levelManager.LoadScene("Main"));
        string currentScene = saveSystem.Load("CurrentLvl");
        StartCoroutine(levelManager.LoadScene(currentScene));
        StartCoroutine(levelManager.LoadScene("Doors"));
        // int currentSceneIndex = levelManager.trainCartNameToIndexMap[currentScene];
        //Only load current lvl?? 

        //If the OneTimeTriggers dont get turned of correctly this is the problem!!
        Debug.Log("LoadStart");
        yield return new WaitUntil(() => levelManager.leavelLoadingAmount <= 0);
        Debug.Log("LoadDone");
        yield return new WaitUntil(() => FindObjectsByType<OneTimeTriggers>(FindObjectsSortMode.None) != null);
        FindObjects();
        OneTimeTriggers[] eventTriggers = FindObjectsByType<OneTimeTriggers>(FindObjectsSortMode.None);
        string[] ids = saveSystem.LoadArray("TriggerIds").AsStringArray();

        //Wanted to use maps but this saving system plugin didnt support any type of maps
        foreach (var trigger in eventTriggers)
        {
            foreach (var id in ids)
            {
                if (trigger.id == id)
                {
                    trigger.GetComponent<Collider>().enabled = false;
                }
            }
        }
        
        //Also save camera rotation
        sub.SetPositionAndRotation(saveSystem.Load("SubPos").AsVector3(), saveSystem.Load("SubRot").AsQuaternion());
        player.GetComponent<KinematicCharacterMotor>().SetPositionAndRotation(saveSystem.Load("PlayerPos").AsVector3(), saveSystem.Load("PlayerRot").AsQuaternion());
        damageSystem.shipHp = saveSystem.Load("SubHp").AsFloat();

        PlayerState _state = PlayerState.OUTSIDE_SUBMARINE;
        if (!saveSystem.Load("AirlockOpen").AsBool())
        {
            _state = PlayerState.INSIDE_SUBMARINE;
        }
        gameManager.SetPlayerState(_state);

        loaded = true;
        yield return null;   
    }

    void PlayerRespawn()
    {
        damageSystem.shipHp = saveSystem.Load("SubHp").AsFloat();
    }
    void SubmarineRespawn()
    {
        damageSystem.shipHp = damageSystem.startShipHp;
        // damageSystem.shipHp = saveSystem.Load("SubHp").AsFloat();
    }

    public IEnumerator DevMenuLoadLvl(Vector3 _subPos, Quaternion _subRot, Vector3 _playerPos, Quaternion _playerRot, string[] _ids, bool _inSub, string _loadedLvl1, string _loadedLvl2 = "")
    {
        FindObjects();
        saveSystem.Save("TriggerIds", _ids);
        // yield return new WaitForSeconds(2);
        FindObjects();
        StartCoroutine(DevMenuLoadingLevel(_subPos, _subRot, _playerPos, _playerRot, _ids, _inSub, _loadedLvl1, _loadedLvl2));
        yield return null;

    }
    IEnumerator DevMenuLoadingLevel(Vector3 _subPos, Quaternion _subRot, Vector3 _playerPos, Quaternion _playerRot, string[] _ids, bool _inSub, string _loadedLvl1, string _loadedLvl2)
    {
        StartCoroutine(levelManager.LoadScene("Main"));
        StartCoroutine(levelManager.LoadScene(_loadedLvl1));
        if (_loadedLvl2 != "")
        {
            StartCoroutine(levelManager.LoadScene(_loadedLvl2));
        }
        StartCoroutine(levelManager.LoadScene("Doors"));

        yield return new WaitUntil(() => levelManager.leavelLoadingAmount <= 0);
        Debug.Log("LoadingDone");
        yield return new WaitUntil(() => FindObjectsByType<OneTimeTriggers>(FindObjectsSortMode.None) != null);
        Debug.Log("LoadingFindAllObjectsDone!");

        //Disable img 
        uiImage.gameObject.SetActive(false);

        FindObjects();
        OneTimeTriggers[] eventTriggers = FindObjectsByType<OneTimeTriggers>(FindObjectsSortMode.None);

        foreach (var trigger in eventTriggers)
        {
            foreach (var id in _ids)
            {
                if (trigger.id == id)
                {
                    trigger.GetComponent<Collider>().enabled = false;
                }
            }
        }

        sub.SetPositionAndRotation(_subPos, _subRot);
        player.GetComponent<KinematicCharacterMotor>().SetPositionAndRotation(_playerPos, _playerRot);

        PlayerState _state = PlayerState.OUTSIDE_SUBMARINE;
        if (_inSub)
        {
            _state = PlayerState.INSIDE_SUBMARINE;
        }
        gameManager.SetPlayerState(_state);

        yield return null;
    }
}
