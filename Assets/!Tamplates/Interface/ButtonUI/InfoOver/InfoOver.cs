using System;
using System.Xml.Linq;
using UnityEngine;

public class InfoOver : MonoBehaviour
{
    public static InfoOverView infoOverView;

    [SerializeField] private InfoOverData overData;

    void Start()
    {
        if (infoOverView == null)
            infoOverView = FindFirstObjectByType<InfoOverView>();
    }

    public void OverrideInfoData (InfoOverData overData)
    {
        this.overData = overData;
    }

    public void OverInfo(Vector3 pointOnScreen)
    {
        infoOverView.Show(overData, pointOnScreen);
    }

    public void UnOverInfo()
    {
        infoOverView.Hide(overData);
    }

}

[Serializable]
public class InfoOverData
{
    public string ID;
    public string Name;
    [TextArea] public string Description;

    public InfoOverData (InfoOverData infoOverData)
    {
        ID = infoOverData.ID;
        Name = infoOverData.Name;
        Description = infoOverData.Description;
    }

    public InfoOverData(string iD, string name, string description)
    {
        ID = iD;
        Name = name;
        Description = description;
    }
}
