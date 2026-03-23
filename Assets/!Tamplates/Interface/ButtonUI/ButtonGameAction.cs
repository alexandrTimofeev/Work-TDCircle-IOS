using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGameAction : MonoBehaviour
{
    public ButtonGameActionInfo ActionInfo;

    private void Start()
    {
        Init(GameG.ButtonGameMediator);
    }

    public void Init (ButtonGameActionMediator mediator)
    {

        GetComponent<Button>().onClick.AddListener(() => mediator.Invoke(ActionInfo));
    }
}

public enum ButtonGameActionType { None, Delite, Info }
[Serializable]
public class ButtonGameActionInfo
{
    public ButtonGameActionType ActionType;
}
