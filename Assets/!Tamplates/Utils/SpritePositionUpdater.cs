using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePositionUpdater : MonoBehaviour
{
    [SerializeField] private float offset = 0f;
    [SerializeField] private float coef = 1f;
    [SerializeField] private bool isAlways = true;

    void Update()
    {
        if (isAlways)
            UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.y * coef) + offset);
    }
}
