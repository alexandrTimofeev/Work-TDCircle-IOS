using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBuildMenu : MonoBehaviour
{
    [SerializeField] private RectTransform selectTr;
    [SerializeField] private UIBuildButton buttonPref;
    [SerializeField] private RectTransform content;
    private List<UIBuildButton> buttonMenu = new List<UIBuildButton>();
    private Dictionary<UIBuildButton, int> buildCount = new Dictionary<UIBuildButton, int>();

    [Space]
    [SerializeField] private GameObject buttonCancelUI;
    [SerializeField] private GameObject buttonCreateUI;

    private BuildDataPreset currentPreset;
    private UIBuildButton currentSelect;

    public void Init(BuildDataPreset[] presets)
    {
        foreach (var preset in presets)
        {
            UIBuildButton button = Instantiate(buttonPref, content);
            buttonMenu.Add(button);
            buildCount.Add(button, 0);

            InitButton(button, preset);
        }
        UpdateButtonsData();

        GameG.ResourceManager.OnChangeResource += (container) => UpdateButtonsData();
        GameG.BuildManager.OnBuild += BuildWork;
    }

    private void InitButton (UIBuildButton button, BuildDataPreset preset)
    {
        button.Init(preset);
        button.OnClickButton += SelectButton;
        button.OnChangeInteractable += (interact) => ChangeButtonInteractWork(button, interact);

    }

    public void SelectButton (UIBuildButton button)
    {
        selectTr.gameObject.SetActive(true);
        selectTr.position = button.transform.position;
        currentPreset = button.GetData();
        currentSelect = button;

        GameG.BuildManager.StartBuildState(currentPreset.BuildObject, GameG.Planet, currentPreset.BuildRule);
    }

    public void Unselect()
    {
        GameG.BuildManager.SetStateOff();
        DOVirtual.DelayedCall(Time.deltaTime, () => currentSelect = null);
    }

    public void SetBuildState(BuildPlanetState state)
    {
        switch (state)
        {
            case BuildPlanetState.Off:
                selectTr.gameObject.SetActive(false);
                buttonCancelUI.gameObject.SetActive(false);
                buttonCreateUI.gameObject.SetActive(false);
                break;
            case BuildPlanetState.SelectToPlace:
                selectTr.gameObject.SetActive(true);
                buttonCancelUI.gameObject.SetActive(true);
                buttonCreateUI.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void UpdateButtonsData()
    {
        foreach (var button in buttonMenu)
        {
            button.UpdateData(buildCount[button]);
        }
    }

    private void ChangeButtonInteractWork(UIBuildButton button, bool interact)
    {
        if (!interact && button == currentSelect)
            Unselect();
    }

    private void BuildWork(BuildbleObject build)
    {
        GameG.ResourceManager.RemoveResourcesFromCondition(currentPreset.resourceConditions,
            currentSelect ? buildCount[currentSelect] : 0);
        if (currentSelect)
            buildCount[currentSelect]++;

        UpdateButtonsData();
    }
}

[Serializable]
public class BuildDataPreset
{
    public string ID;
    public Sprite Icon;
    public string Name;
    public string Description;

    [Space]
    public BuildbleObject BuildObject;
    public BuildRule BuildRule;

    [Space]
    public BuildResourceCondition[] resourceConditions;

    public bool TestRecourceData(Dictionary<string, IntContainer> resources, int countBuild)
    {
        foreach (var condition in resourceConditions)
        {
            if (!resources.ContainsKey(condition.Title))
                return false;
            if (resources[condition.Title].Value < condition.GetCount(countBuild))
                return false;
        }
        return true;
    }

    public int GetResourceCondition(string rTitle, int count = 0)
    {
        foreach (var condition in resourceConditions)
        {
            if (condition.Title == rTitle)
                return condition.GetCount(count);
        }
        return 0;
    }

    [Serializable]
    public struct BuildResourceCondition
    {
        public string Title;
        public int Count;
        public float PowerUp;

        public int GetCount(int countBuild)
        {
            return (int)(Count + (Mathf.Pow(Count, countBuild * PowerUp) - 1));
        }
    }

    public bool TestData(int count)
    {
        return TestRecourceData(GameG.ResourceManager.MyResources, count);
    }
}
