using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public enum BuildPlanetState { Off, SelectToPlace }
public class BuildOnPlanetManager
{
    [SerializeField] private BuildVisualistor visualistor;

    private BuildPlanetState state;
    private Vector3 posCurrsor;

    private BuildbleObject currentBuildbleObject;
    private BuildRule currentRule;
    private PlanetPositor currentPlanet;
    private int currentZone;
    private float currentPositionInOrbit;

    public BuildPlanetState State => state;
    private IInput input;

    public event Action<BuildPlanetState> OnStateChange;
    public event Action<BuildbleObject> OnBuild;

    public void Init (IInput input)
    {
        this.input = input;
        input.OnMoved += MovedWork;

        visualistor = UnityEngine.Object.FindFirstObjectByType<BuildVisualistor> (FindObjectsInactive.Include);
    }

    public void Clear()
    {
        input.OnMoved -= MovedWork;
    }

    public void StartBuildState(BuildbleObject buildbleObject, PlanetPositor planet, BuildRule rule)
    {
        state = BuildPlanetState.SelectToPlace;
        currentBuildbleObject = buildbleObject;
        currentRule = rule;
        currentPlanet = planet;

        visualistor.gameObject.SetActive(true);
        MovedWork(new IInput.InputMoveScreenInfo() { PositionOnScreen = posCurrsor });

        OnStateChange?.Invoke(state);
    }

    private void MovedWork(IInput.InputMoveScreenInfo info)
    {
        posCurrsor = info.PositionOnScreen;

        SetVisualisatorPosition(Camera.main.ScreenToWorldPoint(posCurrsor));
    }

    private void SetVisualisatorPosition(Vector2 positionInWorld)
    {
        if (currentRule == null)
            return;

        if (currentRule.CanPlaceNotZone)
            visualistor.transform.position = positionInWorld;
        else
            visualistor.transform.position = currentPlanet.WorldToOrbitPosition(positionInWorld, out currentZone, out currentPositionInOrbit, 
                currentRule.MinMaxZone.x, currentRule.MinMaxZone.y);
    }

    public void ClickBuild()
    {
        if (state != BuildPlanetState.SelectToPlace)
            return;

        Build();
    }

    private void Build()
    {
        SetStateOff();

        BuildbleObject buildbleObject = UnityEngine.Object.Instantiate(currentBuildbleObject, visualistor.transform.position, 
            Quaternion.identity);
        buildbleObject.OnBuildInit(currentZone, currentPositionInOrbit, visualistor.transform.position, currentPlanet);

        OnBuild?.Invoke(buildbleObject);
    }

    public void SetStateOff()
    {
        state = BuildPlanetState.Off;
        visualistor.gameObject.SetActive(false);

        OnStateChange?.Invoke(state);
    }
}

[Serializable]
public class BuildRule
{
    public Vector2Int MinMaxZone = new Vector2Int(-1, 100);
    public bool CanPlaceNotZone = false;
}