using System;
using UnityEngine;

[ExecuteAlways]
public class PlanetTransform : MonoBehaviour
{
    public PlanetPositor Planet;
    public int Zone = 1;
    [Range(0, 360)]
    public float Position = 0f;
    public float Offset = 0f;

    [Space]
    [SerializeField] private Transform lookDirectionMoveTr;

    [Space]
    [SerializeField] private bool isLookOutCenter;

    [Space]
    public bool IsAutoUpdate = false;
    public bool IsEditorUpdate = false;

    void Start()
    {
        if (IsAutoUpdate && Planet == null)
            Planet = FindFirstObjectByType<PlanetPositor>();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            if (IsEditorUpdate)
                UpdatePosition();

            return;
        }

        if (!IsAutoUpdate)
            return;

        UpdatePosition();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!IsEditorUpdate)
            return;

        if (Planet == null)
            Planet = FindFirstObjectByType<PlanetPositor>();

        UpdatePosition();
    }
#endif

    public void UpdatePosition()
    {
        if (Planet == null)
            return;

        Vector2 direction = transform.position;

        Vector2 posInWorld = Planet.GetPosition(Position, Zone, Offset);
        transform.position = posInWorld;

        direction = (posInWorld - direction).normalized;
        if (lookDirectionMoveTr && direction != Vector2.zero)
            lookDirectionMoveTr.transform.rotation = Quaternion.LookRotation(direction, -Vector3.forward);

        Position = PositionClamp(Position);

        if (isLookOutCenter)
            transform.rotation = Quaternion.LookRotation(transform.forward, transform.position - Planet.transform.position);
    }

    public static float PositionClamp (float position)
    {
        while (position >= 360)
            position -= 360;

        while (position < 0)
            position += 360;

        return position;
    }

    public void AddPositionInDistance(float dist)
    {
        float perimetr = Planet.GetPerimetrZone(Zone, Offset);
        float degreesPerUnit = 360f / perimetr;  // сколько градусов в одной единице расстояния
        Position += dist * degreesPerUnit;        // прибавляем расстояние, переведённое в градусы
    }
}