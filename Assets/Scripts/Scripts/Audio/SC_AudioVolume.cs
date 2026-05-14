using System;
using UnityEngine;

public class SC_AudioVolume : MonoBehaviour
{
    [NonSerialized] private readonly Collider collider; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        //other.gameObject.CompareTag();
    }
}
