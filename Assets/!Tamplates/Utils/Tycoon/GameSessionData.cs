using System;
using UnityEngine;

public class GameSessionData : MonoBehaviour
{
    public IntContainer LifeContainer;

    public Action<IntContainer> OnChangeResource;

    public void Init(ReadOnlyGameData gameGlobalData)
    {
        //LifeContainer = gameGlobalData.LifeContainer.CloneForEdit();

        OnChangeResource = null;
    }
}