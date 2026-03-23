using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Gradient gradient;

    public void RandomColor()
    {
        SetColor(Random.ColorHSV());
    }

    public void RandomColorGradient ()
    {
        SetColor(gradient.Evaluate(Random.Range(0f, 1f)));
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
