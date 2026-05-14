//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//
//public class TrainCartLoader : MonoBehaviour
//{
//    static TrainCartLoader instance;
//    public List<string> trainCartScenesNames;
//    Dictionary<string, int> trainCartNameToIndexMap = new();
//
//    public static TrainCartLoader GetInstance()
//    {
//        return instance;
//    }
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Awake()
//    {
//        if (instance != null)
//        {
//            Destroy(this);
//            return;
//        }
//        instance = this;
//        for (int i = 0; i < trainCartScenesNames.Count; i++)
//            trainCartNameToIndexMap[trainCartScenesNames[i]] = i;
//    }
//    private void Start()
//    {
//        SaveSystem.TrainCartLoaderLoaded();
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//        
//    }
//    void LoadScene(string scene)
//    {
//        if (!SceneManager.GetSceneByName(scene).IsValid())
//        {
//            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
//        }
//    }
//    void UnloadScene(string scene)
//    {
//        if (SceneManager.GetSceneByName(scene).IsValid())
//        {
//            SceneManager.UnloadSceneAsync(scene);
//            //Debug.Log("Unloaded " + scene);
//        }
//    }
//    public void LoadForCart(string scene)
//    {
//        for (int i=0; i< trainCartScenesNames.Count;i++)
//        {
//            UnloadScene(trainCartScenesNames[i]);
//        }
//        int CartIndex = trainCartNameToIndexMap[scene];
//        LoadScene(trainCartScenesNames[CartIndex]);
//        if (CartIndex - 1 >= 0)
//        {
//            LoadScene(trainCartScenesNames[CartIndex - 1]);
//        }
//        if (CartIndex + 1 < trainCartScenesNames.Count)
//        {
//            LoadScene(trainCartScenesNames[CartIndex + 1]);
//        }
//    }
//    public void EnteredCart(CartBox cart)
//    {
//        int CartIndex = trainCartNameToIndexMap[cart.gameObject.scene.name];
//        if (CartIndex - 2 >= 0)
//        {
//            UnloadScene(trainCartScenesNames[CartIndex - 2]);
//        }
//        if (CartIndex - 1 >= 0)
//        {
//            LoadScene(trainCartScenesNames[CartIndex - 1]);
//        }
//        if (CartIndex + 1 < trainCartScenesNames.Count)
//        {
//            LoadScene(trainCartScenesNames[CartIndex + 1]);
//        }
//        if (CartIndex + 2 < trainCartScenesNames.Count)
//        {
//            UnloadScene(trainCartScenesNames[CartIndex + 2]);
//        }
//    }
//}
