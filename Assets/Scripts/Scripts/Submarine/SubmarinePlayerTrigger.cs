using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SubmarinePlayerTrigger : MonoBehaviour
{
    public GameObject[] meshes;
    public CharacterCameraComponent characterCamera;
    public CharacterControllerComponent character;

    [Header("OUTSIDE VOLUME")]
    public Volume outsideVolume;
    public VolumeProfile outsideProfileIN;
    public VolumeProfile outsideProfileOUT;


    [Header("INSIDE VOLUME")]
    public Volume insideVolume;
    public VolumeProfile insideProfileIN;
    public VolumeProfile insideProfileOUT;

    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Player"))
        {
            ChangeToInside();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Contains("Player"))
        {
            ChangeToOutside();
        }
    }

    public void ChangeToOutside()
    {
        foreach (var mesh in meshes)
        {
            mesh.layer = 7;
        }

        characterCamera.isOnSubmarine = false;


        if (insideVolume && insideProfileOUT && insideProfileIN)
        {
            insideVolume.profile = insideProfileOUT;
        }

        if (outsideVolume && outsideProfileOUT && outsideProfileIN)
        {
            outsideVolume.profile = outsideProfileOUT;
        }
    }

    public void ChangeToInside()
    {
        foreach (var mesh in meshes)
        {
            mesh.layer = 6;
        }
        characterCamera.isOnSubmarine = true;

        if (insideVolume && insideProfileOUT && insideProfileIN)
        {
            insideVolume.profile = insideProfileIN;
        }

        if (outsideVolume && outsideProfileOUT && outsideProfileIN)
        {
            outsideVolume.profile = outsideProfileIN;
        }
    }
}
