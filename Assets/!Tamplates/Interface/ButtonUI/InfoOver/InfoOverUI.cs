using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoOverUI : InfoOver, IPointerEnterHandler, IPointerExitHandler
{
    public void ShowOverUI()
    {
        OverInfo(GetComponent<RectTransform>().position);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OverInfo(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnOverInfo();
    }
}