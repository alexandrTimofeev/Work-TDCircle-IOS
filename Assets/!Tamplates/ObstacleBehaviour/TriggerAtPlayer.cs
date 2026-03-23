using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class TriggerAtPlayer : MonoBehaviour
{
    [SerializeField] private string id;
    public string ID => id;

    [SerializeField] private AtPlayerAction[] atPlayerActions;

    public event Action OnPlayerEnter;
    public UnityEvent OnPlayerEnterEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player2DActionBase player))
        {
            foreach (var playerAction in atPlayerActions)
            {
                switch (playerAction.action)
                {
                    case AtPlayerActionType.None:
                        break;
                    case AtPlayerActionType.AddForce:
                        player.GetComponent<Rigidbody2D>().AddForce(playerAction.force, ForceMode2D.Impulse);
                        break;
                    default:
                        break;
                }
            }

            OnPlayerEnter?.Invoke();
            OnPlayerEnterEvent?.Invoke();
        }
    }

    public static TriggerAtPlayer FindWithID (string id)
    {
        TriggerAtPlayer[] triggerAtPlayers = GameObject.FindObjectsOfType<TriggerAtPlayer>();
        foreach (var item in triggerAtPlayers)
        {
            if (item.ID == id)           
                return item;            
        }
        return null;
    }
}

public enum AtPlayerActionType { None, AddForce };
[System.Serializable]
public class AtPlayerAction
{
    public string title;
    public AtPlayerActionType action;
    public Vector3 force;
}