using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public Dictionary<string, IntContainer> MyResources = new Dictionary<string, IntContainer>();
    private Dictionary<string, float> powers = new Dictionary<string, float>();

    public event Action<IntContainer> OnChangeResource;

    public void Init()
    {
        foreach (var resource in G.GlobalData.Resources)
        {
            SubsribeResource(resource);
        }     
    }

    private void SubsribeResource(ReadOnlyIntContainer resource)
    {
        IntContainer container = resource.CloneForEdit();
        MyResources.Add(resource.Title, container);
        SubsribeContainer(container);

        powers.Add(resource.Title, 1f);
    }

    private void SubsribeContainer(IntContainer container, bool isCreateFlyingText = true)
    {
        container.OnChangeValue += (value) => InterfaceManager.BarMediator.ShowForID(container.Title, value);
        container.OnChangeValue += (value) => OnChangeResource?.Invoke(container);
        if (isCreateFlyingText)
        {
            BarUI[] bars = InterfaceManager.BarMediator.GetBarsID(container.Title);
            foreach (BarUI bar in bars)
                CreateFlyingText(container.Value, "", bar.transform.position);
        }

        container.UpdateValue();
    }

    public void CreateFlyingText (int valueDelta, string resTxt, Vector3 point)
    {
        string p = valueDelta > 0 ? "<Color=green>+" : (valueDelta == 0 ? "<Color=grey>+" : "<Color=red>");
        TextScoreUpLR textScoreUpLR = InterfaceManager.CreateFlyingText($"{p}{valueDelta}</Color> {resTxt}", Color.white, point, null);

        //textScoreUpLR.transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
        textScoreUpLR.transform.DOScale(1f, 15f);
    }

    public void RemoveResourcesFromCondition(BuildDataPreset.BuildResourceCondition[] resourceConditions, int countBuild)
    {
        foreach (var condition in resourceConditions)
        {
            if (MyResources.ContainsKey(condition.Title))
                MyResources[condition.Title].RemoveValue(condition.GetCount(countBuild));
        }
    }

    public void AddResource(string title, int add, Vector3 position)
    {
        add = (int)(add * powers[title]);

        MyResources[title].AddValue(add);
        CreateFlyingText(add, title + " ", position);
    }

    public void RemovePower(string title, float remove)
    {
        powers[title] -= remove;
    }

    public void AddPower(string title, float add)
    {
        powers[title] += add;
    }

    /*
    public bool IsCanBuy(BuyData buyData)
    {
        return EnergyContainer.Value >= buyData.GetRealPrice();
    }*/
}