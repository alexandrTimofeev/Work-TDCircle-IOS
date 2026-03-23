using System;
using TMPro;
using UnityEngine;

public class InfoOverView : MonoBehaviour
{
    [SerializeField] private RectTransform overRect;
    [SerializeField] private Vector2 sizeOverZone;
    [SerializeField] private TextMeshProUGUI tmp;

    private InfoOverData overData;

    public void Show(InfoOverData overData, Vector3 pointOnScreen)
    {
        Hide(overData);
        overRect.gameObject.SetActive(true);

        float offsetX = sizeOverZone.x;
        float offsetY = sizeOverZone.y;

        if ((pointOnScreen.x + offsetX) > Screen.width || (pointOnScreen.x + offsetX) < 0)
            offsetX -= offsetX;

        if ((pointOnScreen.y + offsetY) > Screen.height || (pointOnScreen.y + offsetY) < 0)
            offsetY -= offsetY;

        overRect.localPosition = new Vector3(offsetX, offsetY, 0f);
        tmp.text = overData.Description;

        transform.position = pointOnScreen;

        this.overData = overData;   
    }

    public void Hide(InfoOverData overData)
    {
        if (overData == null)
            return;

        overData = null;
        overRect.gameObject.SetActive(false);
    }
}
