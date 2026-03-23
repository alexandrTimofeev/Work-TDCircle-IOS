using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GridUniversalInteractable3D : UniversalInteractable3D, IGridObject
{
    [SerializeField] private CellPlacer cellPlacer;
    [SerializeField] private bool isOnlyEmptyCell = true;
    [SerializeField] private bool isPutInCellPlacer = true;
    [SerializeField] private Vector2Int[] myPlaces;

    private Vector2Int posintionInGrid;

    public Vector2Int CellPosition => posintionInGrid;

    public Vector2Int[] MyPlaces => myPlaces;

    public Action<IGridObject, Vector2Int> OnMove;
    public Action<IGridObject, Vector2Int> OnPutInPlace;
    public Action<IGridObject> OnDeliteUniversalGrid;


    public void InitGrid(CellPlacer cellPlacer)
    {
        this.cellPlacer = cellPlacer;
        PutInPlace(cellPlacer.WorldToCell(transform.position).Value);
    }

    public override void Drag(Vector3 hitPoint)
    {
        if (!dragEnable)
            return;

        if (cellPlacer == null)
        {
            base.Drag(hitPoint);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 planeHit = ray.GetPoint(enter);
            Vector2Int? posInGrid = cellPlacer.WorldToCell(planeHit);
            if (posInGrid.HasValue &&
                (isOnlyEmptyCell == false || cellPlacer.CanPlaceObject(this, posInGrid.Value)))
                PutInPlace(posInGrid.Value);
        }
    }

    private void PutInPlace(Vector2Int posInGrid)
    {
        bool canMove = true;
        if (isPutInCellPlacer)        
            canMove = cellPlacer.MoveObject(this, posInGrid);        

        if (canMove == false)
            return;

        transform.position = cellPlacer.CellToWorld(posInGrid);
        posintionInGrid = posInGrid;

        OnPutInPlace?.Invoke(this, posInGrid);
    }

    public void OnPlaced(CellPlacer placer, Vector2Int position)
    {
        posintionInGrid = position;

        OnPutInPlace?.Invoke(this, position);
        OnMove?.Invoke(this, position);
    }

    public void OnRemoved(CellPlacer placer, Vector2Int position)
    { }    

    public void OnMoved(CellPlacer placer, Vector2Int from, Vector2Int to)
    {
        posintionInGrid = to;

        OnPutInPlace?.Invoke(this, to);
        OnMove?.Invoke(this, to);
    }

    public IEnumerator MoveAnimationProcess(Vector2Int cellStart, Vector2Int cellEnd)
    {
        yield break;
    }

    public IEnumerator CreationProcess()
    {
        yield break;
    }

    public IEnumerator CalculateProcess(GridObjectCalcualteData calcualteData)
    {
        yield break;
    }

    public void UpdatePos()
    {
        PutInPlace(posintionInGrid);
    }

    void IGridObject.Delite()
    {
        OnDeliteUniversalGrid?.Invoke(this);
        cellPlacer.RemoveObject(CellPosition);
        Destroy(gameObject);
    }
}