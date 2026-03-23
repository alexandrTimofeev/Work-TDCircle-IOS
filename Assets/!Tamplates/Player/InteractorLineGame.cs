using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class InteractorLineGame : MonoBehaviour
{
    [Space]
    [SerializeField] private Transform selectStartVisual;
    [SerializeField] private Transform selectEndVisual;

    [Space]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioDataPlay selectDataPlay;
    [SerializeField] private AudioDataPlay unselectDataPlay;
    [SerializeField] private AudioDataPlay tapDataPlay;
    [SerializeField] private AudioDataPlay wrongDataPlay;

    private InteractorDoData interactorData = new InteractorDoData();
    private bool isInteractionWait = false;
    private bool isInteractionReady = false;

    private IInput input;

    public void Init (IInput input)
    {
        this.input = input;
        this.input.OnBegan += OnTap;
    }

    public IEnumerator WaitPlayerInteraction()
    {
        Debug.Log($"Player WaitPlayerInteraction");
        SetStateInteractionActive(true);

        yield return new WaitUntil(() => isInteractionReady);

        isInteractionReady = false;
        SetStateInteractionActive(false);
        //GameG.Main.AddPlayerInteractionData(interactorData);
        ClearSelect();
    }

    private void SetStateInteractionActive(bool v)
    {
        isInteractionWait = v;
    }

    private void OnTap(Vector2 point)
    {
        point = Camera.main.ScreenToWorldPoint(point);

        if (!isInteractionWait)
            return;

        Vector2Int? cellTest = GameG.CellPlacer.WorldToCell(point);
        Debug.Log($"OnTap {(cellTest != null ? cellTest.Value : "null")}");
        if (!cellTest.HasValue)
            return;

        if (TryTapStartSelect(cellTest.Value, out IGridObject gridObject))
        {
            interactorData.cellStart = cellTest;
            interactorData.GridObjectStart = gridObject;
            SelectStart(interactorData.cellStart);
        }
        else if (interactorData.cellStart != null)
        {
            if (TryTapPathSelect(interactorData.cellStart.Value, cellTest.Value, out List<Vector2Int> path))
            {
                interactorData.cellEnd = cellTest;
                interactorData.path = path;
                audioSource.Play(tapDataPlay);
                isInteractionReady = true;
            }
            else
            {
                audioSource.Play(wrongDataPlay);
            }
        }
        else
        {
            ClearSelect();
        }
    }

    private void ClearSelect()
    {
        interactorData = new InteractorDoData();
        SelectStart(null);
        SelectEnd(null);
    }

    private bool TryTapStartSelect(Vector2Int cell, out IGridObject gridObject)
    {
        gridObject = GameG.CellPlacer.GetObject(cell);
        return gridObject != null;
    }

    private bool TryTapPathSelect(Vector2Int cellStart, Vector2Int cellEnd, out List<Vector2Int> path)
    {
        path = GameG.CellPlacer.FindPath(cellStart, cellEnd, true);
        return path != null;
    }

    private void SelectStart(Vector2Int? cellStart)
    {
        if (cellStart == null)
        {
            selectStartVisual.gameObject.SetActive(false);
            if(interactorData.cellStart != null)
                audioSource.Play(unselectDataPlay);
            return;
        }

        selectStartVisual.gameObject.SetActive(true);
        selectStartVisual.position = GameG.CellPlacer.CellToWorld(cellStart.Value);

        audioSource.Play(selectDataPlay);
    }

    private void SelectEnd(Vector2Int? cellEnd)
    {
        if (cellEnd == null)
        {
            selectEndVisual.gameObject.SetActive(false);
            return;
        }

        selectEndVisual.gameObject.SetActive(true);
        selectEndVisual.position = GameG.CellPlacer.CellToWorld(cellEnd.Value);
    }

    private void OnDestroy()
    {
        this.input.OnBegan -= OnTap;
    }
}

[Serializable]
public class InteractorDoData
{
    public Vector2Int? cellStart;
    public Vector2Int? cellEnd;

    public IGridObject GridObjectStart = null;
    public IGridObject GridObjectEnd = null;

    public List<Vector2Int> path = new List<Vector2Int>();

    public void Clear()
    {
        cellStart = null;
        cellEnd = null;
        GridObjectStart = null;
        GridObjectEnd = null;
        path.Clear();
    }
}