using System.Collections.Generic;
using UnityEngine;

public class SC_ActivationManger : MonoBehaviour
{
    public static SC_ActivationManger _instance { get; private set; }

    private void Awake()
    {
        if( _instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
    }

    public void ActiveTheseGameObjects(List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects) { 
            gameObject.SetActive(true);
        }
    }

    public void DeactiveTheseGameObjects(List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }
    }
}