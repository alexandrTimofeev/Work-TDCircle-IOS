using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchivInfoWindowUI : WindowUI
{
    [Space]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textInfo;
    private AchivInfo achivInfo;

    [Space]
    [SerializeField] private bool useTitleInTextInfo;
    [SerializeField] private string titlePrefix = "<color=yellow><size=25<nobr>";
    [SerializeField] private string titlePostfix = "</color></size></nobr>";

    public override void Open()
    {
        base.Open();
    }

    public void Init(AchivInfo achiv, Achiviement[] allAchiviements = null, bool? forceTestCondition = null)
    {
        achivInfo = achiv;
        bool contains = forceTestCondition != null ? forceTestCondition.Value : 
            AchieviementSystem.IsUnlockAchiv(achiv.ID, false, allAchiviements);

        achivInfo.ApplyToImage(image, contains);
        if(useTitleInTextInfo)
            textInfo.text = $"{titlePrefix}{achiv.Title}{titlePostfix}\n\n";
        
        textInfo.text += achiv.InfoTxt;
    }
}
