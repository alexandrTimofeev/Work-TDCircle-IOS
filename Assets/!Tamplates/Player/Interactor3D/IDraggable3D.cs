using UnityEngine;

public interface IDraggable3D
{
    bool IsEnable();

    void BeginDrag(Vector3 hitPoint);
    void Drag(Vector3 hitPoint);
    void EndDrag();
}