using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Checkpoint : MonoBehaviour
{
    public Action<Checkpoint> OnCheck;

    public void Check()
    {
        Debug.Log($"{gameObject.name} check save");
        OnCheck?.Invoke(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player2DActionBase>(out Player2DActionBase player))
        {
            Check();
        }
    }
}
