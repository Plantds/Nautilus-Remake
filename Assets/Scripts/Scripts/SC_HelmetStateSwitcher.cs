using System;
using UnityEngine;

public class SC_HelmetStateSwitcher : MonoBehaviour, IStateChanger
{
    [SerializeField] private GameObject helmet;

    public void ChangeState(PlayerState _state)
    {
        switch(_state)
        {
            case PlayerState.INSIDE_SUBMARINE:
            {
                helmet.SetActive(false);
                break;
            }
            case PlayerState.OUTSIDE_SUBMARINE:
            {
                helmet.SetActive(true);
                break;
            }
        }
    }
}
