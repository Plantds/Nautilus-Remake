using System.Collections.Generic;
using UnityEngine;

public class SubmarineAirlock : MonoBehaviour
{
    [Header("Editor Tools")]
    [SerializeField] private bool forceRefresh = false;

    [SerializeField] public List<GameObject> meshes;
    [SerializeField] public List<GameObject> LightOutside;
    [SerializeField] public List<GameObject> LightInside;

    private void OnValidate()
    {
        forceRefresh = false;
        meshes.Clear();

        FindMeshRendererChildren(this.gameObject);
    }

    private void FindMeshRendererChildren(GameObject _object)
    {
        MeshRenderer mesh = new MeshRenderer();
        for (int i = 0; i < _object.transform.childCount; i++)
        {
            bool validMesh = false;
            GameObject itr = _object.transform.GetChild(i).gameObject;

            if(itr.tag.Contains("IgnoreAutoFind"))
            {
                continue;
            }

            if (itr.GetComponent<SkinnedMeshRenderer>() || itr.GetComponent<MeshRenderer>() || itr.GetComponent<Light>())
            {
                validMesh = true;
            }

            if (validMesh && (itr.layer != 3 && itr.layer != 1))
            {
                meshes.Add(itr);
            }

            if (itr.transform.childCount > 0)
            {
                FindMeshRendererChildren(itr);
            }
        }
    }

    void Update()
    {
        // airlockSequence.ManualUpdate();

        // switch (airlockSequence.currentAirlockState)
        // {
        //     case AirlockSequenceState.CLOSING:
        //         {

        //             break;
        //         }
        //     case AirlockSequenceState.PRECLOSING:
        //         {
        //             break;
        //         }
        //     case AirlockSequenceState.NEUTRAL:
        //         {
        //             break;
        //         }
        //     case AirlockSequenceState.PREOPENING:
        //         {
        //             if (currentState == layer.OUTSIDE)
        //             {
        //                 AirlockRendererFeature.intensity = 1.0f;
        //                 ChangeLayer(layer.INSIDE);
        //             }
        //             break;
        //         }
        //     case AirlockSequenceState.OPENING:
        //         {
        //             if (currentState == layer.INSIDE)
        //             {
        //                 AirlockRendererFeature.intensity = 0.0f;
        //                 ChangeLayer(layer.OUTSIDE);
        //             }
        //             break;
        //         }
        // }

        //Debug.Log(AirlockRendererFeature.targetHeight);
    }

    public void ChangeLayer(PlayerState _state)
    {
        foreach (var mesh in meshes)
        {
            mesh.layer = (int)_state;
        }

        foreach (var itr in LightOutside)
        {
            itr.SetActive(_state == PlayerState.OUTSIDE_SUBMARINE);
        }

        foreach (var itr in LightInside)
        {
            itr.SetActive(_state == PlayerState.INSIDE_SUBMARINE);
        }

        //airlockSequence.currentWaterState = currentState == layer.OUTSIDE ? SC_SubmarineAirlockSequence.LeverState.INCREASING : SC_SubmarineAirlockSequence.LeverState.DECREASING;
    }

    // public void ChangeStartLayer(layer _layer)
    // {
    //     currentState = _layer;
    //     foreach (var mesh in meshes)
    //     {
    //         mesh.layer = (int)_layer;
    //     }


    //     //airlockSequence.currentWaterState = currentState == layer.OUTSIDE ? SC_SubmarineAirlockSequence.LeverState.INCREASING : SC_SubmarineAirlockSequence.LeverState.DECREASING;
    //     airlockSequence.door.play = currentState == layer.INSIDE;
    //     airlockSequence.ramp.play = currentState == layer.OUTSIDE;

    //     if(currentState == layer.OUTSIDE)
    //     {
    //         airlockSequence.ChangeLightsToOutside();
    //     }
    //     else
    //     {
    //         airlockSequence.ChangeLightsToInside();
    //     }

    //     airlockSequence.currentAirlockState = currentState == layer.OUTSIDE ? AirlockSequenceState.OPENING : AirlockSequenceState.CLOSING;
    // }
}
