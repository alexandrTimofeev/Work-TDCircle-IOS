using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapObjectHide : GrapObject
{
    protected override void DeliteImmidiatly()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (IsActive)
            return;

        gameObject.SetActive(true);
        IsActive = true;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
