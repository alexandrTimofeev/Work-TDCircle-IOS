using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class GridObjectBase : MonoBehaviour, IGridObject
{
    public bool autoFindOnStart = true;

    [Space]
    public bool isChangePositionOnPlace;

    [Space, Header("Animations")]
    public DOTweenScaleData standartCreateDoData;
    public DOTweenPositionData standartMoveDoData;
    public DOTweenPunchData calculateDoData;

    public Vector2Int CellPosition { get; private set; }
    public CellPlacer CellPlacer { get; private set; }

    public Vector2Int[] MyPlaces => null;

    private Tweener tweenerMove;
    private Tweener tweenerCreate;
    private Tweener tweenerCalcualte;

    public virtual void Awake()
    {
        if(autoFindOnStart)
            Init(FindFirstObjectByType<CellPlacer>());
    }

    public void Init(CellPlacer cellPlacer)
    {
        CellPlacer = cellPlacer;
    }

    public virtual void OnPlaced(Vector2Int position)
    {
        OnPlaced(CellPlacer, position);
    }
    public virtual void OnRemoved(Vector2Int position)
    {
        OnRemoved(CellPlacer, position);
    }
    public virtual void OnMoved(Vector2Int from, Vector2Int to)
    {
        OnMoved(CellPlacer, from, to);
    }

    public virtual void OnPlaced(CellPlacer placer, Vector2Int position)
    {
        CellPosition = position;
        if(isChangePositionOnPlace)
            transform.position = CellPlacer.CellToWorld(position);
    }

    public virtual void OnRemoved(CellPlacer placer, Vector2Int position)
    {
        CellPosition = Vector2Int.zero;
    }

    public virtual void OnMoved(CellPlacer placer, Vector2Int from, Vector2Int to)
    {
        CellPosition = to;
        if (isChangePositionOnPlace)
            transform.position = CellPlacer.CellToWorld(to);
    }

    public virtual IEnumerator MoveAnimationProcess(Vector2Int cellStart, Vector2Int cellEnd)
    {
        if (tweenerMove != null)
            tweenerMove.Kill();

        tweenerMove = standartMoveDoData.TransformMove(transform, CellPlacer.CellToWorld(cellEnd)).SetAutoKill(true);

        yield return tweenerMove.WaitForCompletion();
        yield return new WaitForSeconds(standartMoveDoData.durationOnEnd);
    }

    public virtual IEnumerator CreationProcess()
    {
        if (tweenerCreate != null)
            tweenerCreate.Kill();

        transform.localScale = Vector2.zero;
        tweenerCreate = standartCreateDoData.TransformScale(transform, Vector3.one).SetAutoKill(true);

        yield return tweenerCreate.WaitForCompletion();
    }

    public virtual IEnumerator CalculateProcess(GridObjectCalcualteData calcualteData)
    {
        if (tweenerCalcualte != null)
            tweenerCalcualte.Kill();

        bool isAddScore = false;
        tweenerCalcualte = calculateDoData.TransformPunchScale(transform).SetAutoKill(true).OnUpdate(() =>
        {
            if (isAddScore == false)
            {
                CalculateWork(calcualteData);
                isAddScore = true;
            }
        });

        yield return tweenerCalcualte.WaitForCompletion();
    }

    public virtual void CalculateWork (GridObjectCalcualteData calcualteData)
    { }

    public virtual void UpdatePos()
    {
        transform.position = CellPlacer.CellToWorld(CellPosition);
    }

    public virtual void Delite()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        tweenerMove.Kill();
        tweenerCreate.Kill();
        tweenerCalcualte.Kill();
    }

    public void OverridePositionWithoutMove (Vector2Int cellPosition)
    {
        CellPosition = cellPosition;
    }
}

public class GridObjectCalcualteData
{
    public List<GridObjectBase> gridObjectBasesPrev = new List<GridObjectBase>();    
}