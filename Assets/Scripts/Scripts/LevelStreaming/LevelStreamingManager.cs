using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelStreamingManager : MonoBehaviour
{
    static LevelStreamingManager instance;
    public List<string> trainCartScenesNames;
    public Dictionary<string, int> trainCartNameToIndexMap = new();

    [HideInInspector] public int mainReloadingAmount;
    [HideInInspector] public int leavelLoadingAmount;
    [HideInInspector] public int leavelUnloadingAmount;

    public static LevelStreamingManager GetInstance()
    {
        return instance;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        for (int i = 0; i < trainCartScenesNames.Count; i++)
            trainCartNameToIndexMap[trainCartScenesNames[i]] = i;
    }
    private void Start()
    {
        // SaveSystem.TrainCartLoaderLoaded();
        //Delete when connected to menu
        // StartCoroutine(LoadScene(trainCartScenesNames[0]));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator LoadScene(string scene)
    {
        if (!SceneManager.GetSceneByName(scene).IsValid())
        {
            leavelLoadingAmount += 1;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

            yield return new WaitUntil(() => asyncLoad.isDone);
            Debug.Log("LoadLvl: " + scene);

            leavelLoadingAmount -= 1;
            yield return null;
        }
    }

    public IEnumerator UnloadScene(string scene)
    {
        if (SceneManager.GetSceneByName(scene).IsValid())
        {
            leavelUnloadingAmount += 1;

            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(scene);

            yield return new WaitUntil(() => asyncLoad.isDone);

            leavelUnloadingAmount -= 1;
            yield return null;
        }
    }

    public IEnumerator ReloadActiveScene(string scene)
    {
        if (SceneManager.GetSceneByName(scene).IsValid())
        {
            mainReloadingAmount += 1;

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            Resources.UnloadUnusedAssets();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

            yield return new WaitUntil(() => asyncLoad.isDone);

            mainReloadingAmount -= 1;
            yield return null;
        }
    }

    public void LoadForCart(string scene)
    {
        for (int i = 0; i < trainCartScenesNames.Count; i++)
        {
            StartCoroutine(UnloadScene(trainCartScenesNames[i]));
        }
        int CartIndex = trainCartNameToIndexMap[scene];
        StartCoroutine(LoadScene(trainCartScenesNames[CartIndex]));
        if (CartIndex - 1 >= 0)
        {
            StartCoroutine(LoadScene(trainCartScenesNames[CartIndex - 1]));
        }
        if (CartIndex + 1 < trainCartScenesNames.Count)
        {
            StartCoroutine(LoadScene(trainCartScenesNames[CartIndex + 1]));
        }
    }
    public void EnteredCart(LevelSwitchBox cart)
    {
        int CartIndex = trainCartNameToIndexMap[cart.gameObject.scene.name];
        // if (CartIndex - 2 >= 0)
        // {
        //     StartCoroutine(UnloadScene(trainCartScenesNames[CartIndex - 2]));
        // }
        if (CartIndex - 1 >= 0)
        {
            StartCoroutine(UnloadScene(trainCartScenesNames[CartIndex - 1]));
        }
        if (CartIndex + 1 < trainCartScenesNames.Count)
        {
            StartCoroutine(LoadScene(trainCartScenesNames[CartIndex + 1]));
        }
        // if (CartIndex + 2 < trainCartScenesNames.Count)
        // {
        //     StartCoroutine(UnloadScene(trainCartScenesNames[CartIndex + 2]));
        // }
    }
}