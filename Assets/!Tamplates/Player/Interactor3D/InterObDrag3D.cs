using UnityEngine;

public class InterObDrag3D : InterObSelect, IDraggable3D
{
    private Vector3 offset;
    private Plane dragPlane;

    public void BeginDrag(Vector3 hitPoint)
    {
        dragPlane = new Plane(Vector3.up, transform.position);
        offset = transform.position - hitPoint;
    }

    public void Drag(Vector3 hitPoint)
    {
        transform.position = hitPoint + offset;
    }

    public void EndDrag() { }
}