using System;
using UnityEngine;

public class InteractorGame3D : MonoBehaviour
{
    private Camera m_Camera;

    private IOverable3D currentOver;
    private ISelectable3D currentSelect;
    private IDraggable3D currentDrag;

    private GameObject currentDragDebug;

    public bool isActive = true;

    public void Init(IInput input)
    {
        m_Camera = Camera.main;

        input.OnBegan += BeganWork;
        input.OnMoved += MovedWork;
        input.OnEnded += EndedWork;
        input.OnClickAnyPosition += AnyPositionWork;
    }

    private Ray GetRay(Vector2 screenPos)
    {
        return m_Camera.ScreenPointToRay(screenPos);
    }

    private bool Raycast(Vector2 screenPos, out RaycastHit hit)
    {
        return Physics.Raycast(GetRay(screenPos), out hit);
    }

    private bool RaycastIgnoreObject(
    Vector2 screenPos,
    GameObject ignoreObject,
    out RaycastHit hit)
    {
        Ray ray = GetRay(screenPos);

        RaycastHit[] hits = Physics.RaycastAll(ray);
        float minDist = float.MaxValue;
        hit = default;

        foreach (var h in hits)
        {
            if (ignoreObject != null && h.collider.gameObject == ignoreObject)
                continue;

            if (h.distance < minDist)
            {
                minDist = h.distance;
                hit = h;
            }
        }

        return minDist < float.MaxValue;
    }

    public void BeganWork(Vector2 pos)
    {
        if (!isActive)
            return;

        if (!Raycast(pos, out var hit) || hit.rigidbody == null)
        {
            ClearSelection();
            return;
        }

        var clickable = TryClick(hit.rigidbody.gameObject);

        var selectable = TrySelect(hit.rigidbody.gameObject);

        currentDrag = TryGetDrag(hit.rigidbody.gameObject, hit.point);

        if(selectable == null && currentDrag == null)
            ClearSelection();
    }

    public IClickable3D TryClick (GameObject ob)
    {
        var clickable = ob.GetComponent<IClickable3D>();
        clickable?.Click();

        return clickable;
    }

    public ISelectable3D TrySelect(GameObject ob)
    {
        var selectable = ob.GetComponent<ISelectable3D>();
        if (selectable != null)
        {
            currentSelect?.SetSelect(false);
            selectable.SetSelect(true);
            currentSelect = selectable;
        }

        return selectable;
    }

    public IDraggable3D TryGetDrag(GameObject ob, Vector3 point)
    {
        currentDrag = ob.GetComponent<IDraggable3D>();
        currentDrag?.BeginDrag(point);
        currentDragDebug = ob;

        return currentDrag;
    }

    private void MovedWork(IInput.InputMoveScreenInfo info)
    {
        if (!isActive)
            return;

        // -------- OVER / UNOVER (обычный raycast) --------
        IOverable3D newOver = null;

        if (Raycast(info.PositionOnScreen, out var hit))
            newOver = hit.collider.GetComponent<IOverable3D>();

        if (newOver != currentOver)
        {
            currentOver?.UnOver();
            newOver?.Over();
            currentOver = newOver;
        }

        // -------- DRAG (игнорируем перетаскиваемый объект) --------
        if (currentDrag != null)
        {
            if (RaycastIgnoreObject(
                info.PositionOnScreen,
                currentDragDebug,
                out hit))
            {
                currentDrag.Drag(hit.point);
            }
        }
    }

    private void EndedWork(Vector2 pos)
    {
        if (!isActive)
            return;

        currentDrag?.EndDrag();
        currentDrag = null;
    }

    private void AnyPositionWork(Vector2 pos)
    {
        if (!isActive)
            return;

        if (!Raycast(pos, out _) && currentDrag == null)
            ClearSelection();
    }

    private void ClearSelection()
    {
        currentSelect?.SetSelect(false);
        currentSelect = null;
    }

    private void Update()
    {
        if (currentDrag == null)
            currentDragDebug = null;
    }

    public void DeliteCurrent()
    {
        if(currentSelect != null)
        {
            ISelectable3D selectable = currentSelect;
            ClearSelection();
            selectable.Delite();
            (selectable as IGridObject).Delite();
        }
    }
}