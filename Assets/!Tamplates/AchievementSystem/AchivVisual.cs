using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchivVisual : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image[] imagesBack;
    [SerializeField] private TextMeshProUGUI titleTmp;
    private AchivInfo achivInfo;
    private bool contains;

    public event Action<AchivInfo> OnClickOpen;

    public void Init (AchivInfo achivInfo)
    {
        this.achivInfo = achivInfo;

        contains = AchieviementSystem.IsUnlockAchiv(achivInfo.ID, true);
        achivInfo.ApplyToImage(image, contains);

        foreach (var image in imagesBack)
            achivInfo.ApplyToImageBack(image, contains);

        if (titleTmp)
        {
            titleTmp.text = achivInfo.Title;
            titleTmp.color = contains ? Color.yellow : Color.white;
        }
    }   

    public void OpenViewInfo()
    {
        OnClickOpen?.Invoke(achivInfo);
    }
}