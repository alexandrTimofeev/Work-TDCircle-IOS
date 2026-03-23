using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseWindowUI : WindowUI
{
    [SerializeField] private GameObject[] showOnlyInGame;

    public void Show()
    {
        foreach (var go in showOnlyInGame)
        {
            go.SetActive(GameSceneManager.GameSceneName == SceneManager.GetActiveScene().name);
        }
    }

    public override void Close()
    {
        //GameG.GameSessionManagerMono.Pause(false);
        base.Close();
    }
}