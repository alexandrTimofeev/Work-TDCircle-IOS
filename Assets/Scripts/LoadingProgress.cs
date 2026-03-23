using System;
using UnityEngine;

public class LoadingProgress : MonoBehaviour
{
    [SerializeField] private float rotationStep = -500f;
    private void Update()
    {
        gameObject.transform.Rotate(0f, 0f, rotationStep * Time.deltaTime);
    }
}
