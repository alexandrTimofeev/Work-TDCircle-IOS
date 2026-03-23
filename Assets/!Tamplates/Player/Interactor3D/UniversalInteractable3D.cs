using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UniversalInteractable3D :
    MonoBehaviour,
    IOverable3D,
    IClickable3D,
    ISelectable3D,
    IDraggable3D
{
    [Header("Enable flags")]
    [SerializeField] protected bool overEnable = true;
    [SerializeField] protected bool clickEnable = true;
    [SerializeField] protected bool selectEnable = true;
    [SerializeField] protected bool dragEnable = true;

    [Header("Visual")]
    [SerializeField] private Color overColor = Color.yellow;
    [SerializeField] private Color selectColor = Color.green;

    public event Action<IClickable3D> OnOver;
    public event Action<IClickable3D> OnUnOver;
    public event Action<IClickable3D> OnClick;
    public event Action<SelectInfo> OnSelect;

    public event Action<MonoBehaviour> OnDelite;

    [Space]
    [SerializeField] private List<Renderer> rends = new List<Renderer>();
    private Color baseColor;
    protected bool isSelected;

    // Drag
    protected Vector3 dragOffset;
    protected Plane dragPlane;

    private void Awake()
    {
        if(rends.Count == 0)
            rends.Add(GetComponent<Renderer>());
        baseColor = rends[0].material.color;
    }

    // ---------------- ENABLE ----------------

    bool IOverable3D.IsEnable() => overEnable;
    bool IDraggable3D.IsEnable() => dragEnable;

    // ---------------- OVER ----------------

    public void Over()
    {
        if (!overEnable) return;

        if (!isSelected)
            rends.ForEach((rend) => rend.material.color = overColor);

        OnOver?.Invoke(this);
    }

    public void UnOver()
    {
        if (!overEnable) return;

        if (!isSelected)
            rends.ForEach((rend) => {
                if (rend) rend.material.color = baseColor;
            });   

        OnUnOver?.Invoke(this);
    }

    // ---------------- CLICK ----------------

    public void Click()
    {
        if (!clickEnable) return;
        OnClick?.Invoke(this);
    }

    // ---------------- SELECT ----------------

    public bool IsSelect() => isSelected;

    public void SetSelect(bool select)
    {
        if (!selectEnable) return;
        if (isSelected == select) return;

        isSelected = select;
        rends.ForEach((rend) => rend.material.color = select ? selectColor : baseColor);

        OnSelect?.Invoke(new SelectInfo
        {
            Selectable = this,
            IsSelect = select
        });
    }

    // ---------------- DRAG ----------------

    public void BeginDrag(Vector3 hitPoint)
    {
        if (!dragEnable) return;

        // dragPlane по горизонтали через текущую позицию
        dragPlane = new Plane(Vector3.up, transform.position);

        // вычисляем offset как разница между позицией объекта и точкой на плоскости
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 planeHit = ray.GetPoint(enter);
            dragOffset = transform.position - planeHit;
        }
    }

    public virtual void Drag(Vector3 hitPoint)
    {
        if (!dragEnable) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 planeHit = ray.GetPoint(enter);
            transform.position = planeHit + dragOffset;
        }
    }


    public void EndDrag() { }

    void ISelectable3D.Delite()
    {
        OnDelite?.Invoke(this);
        Destroy(gameObject);
    }
}