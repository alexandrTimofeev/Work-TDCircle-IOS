using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LoadingLayoutOffset : MonoBehaviour
{
    [SerializeField] private float bottomOffsetPortrait = 650f;
    [SerializeField] private float bottomOffsetLandscape = 500f;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateOffset();
    }

    private void Update()
    {
        UpdateOffset();
    }

    private void UpdateOffset()
    {
        float offset = Screen.height >= Screen.width ? bottomOffsetPortrait : bottomOffsetLandscape;
        // Устанавливаем нижний отступ через offsetMin
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, offset);
    }
}