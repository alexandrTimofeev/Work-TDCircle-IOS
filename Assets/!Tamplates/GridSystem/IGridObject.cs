using System;
using System.Collections;
using UnityEngine;

public interface IGridObject
{
    public Vector2Int CellPosition { get; }
    public void OnPlaced(CellPlacer placer, Vector2Int position);
    public void OnRemoved(CellPlacer placer, Vector2Int position);
    public void OnMoved(CellPlacer placer, Vector2Int from, Vector2Int to);

    public IEnumerator MoveAnimationProcess(Vector2Int cellStart, Vector2Int cellEnd);
    //public IEnumerator CreationProcess();
    //public IEnumerator CalculateProcess(GridObjectCalcualteData calcualteData);
    public void UpdatePos();
    void Delite();

    public Vector2Int[] MyPlaces { get; }
}