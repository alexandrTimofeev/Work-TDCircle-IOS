using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPositor : MonoBehaviour
{
    [SerializeField] private List<PlanetZoneParametrs> zoneParametrs;
    [SerializeField] private Vector2 center = Vector2.zero;
    public Vector2 CenterZones => (Vector2)transform.position + center;

    [Space]
    [SerializeField] private bool IsDrawZone = false;

    public Vector2 GetPosition(float position, int zone, float offset = 0f)
    {
        if (zoneParametrs == null || zoneParametrs.Count == 0)
            return center;

        zone = Mathf.Clamp(zone, 0, zoneParametrs.Count - 1);

        float radius = GetRadius(zone);

        position = PlanetTransform.PositionClamp(position);

        float angle = position * Mathf.Deg2Rad;

        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        return CenterZones + dir * (radius + offset);
    }

    private float GetRadius(int zone)
    {
        float radius = 0f;

        for (int i = 0; i <= zone; i++)
        {
            radius += zoneParametrs[i].distAtPreviousZone;
        }

        return radius;
    }

    private void OnDrawGizmos()
    {
        if (!IsDrawZone)
            return;

        float radius = 0f;
        for (int i = 0; i < zoneParametrs.Count; i++)
        {
            radius += zoneParametrs[i].distAtPreviousZone;
            Gizmos.DrawWireSphere(CenterZones, radius);
        }
    }

    public Vector3 WorldToOrbitPosition(Vector2 positionOnScreen, out int zone, out float position, int minZone = -1, int maxZone = -1)
    {
        // Если зон нет — возвращаем исходную позицию, помечаем zone = -1
        if (zoneParametrs == null || zoneParametrs.Count == 0)
        {
            zone = -1;
            position = 0f;
            return new Vector3(positionOnScreen.x, positionOnScreen.y, 0f);
        }

        // Нормализуем границы зон: -1 означает отсутствие ограничения
        int min = (minZone < 0) ? 0 : Mathf.Clamp(minZone, 0, zoneParametrs.Count - 1);
        int max = (maxZone < 0) ? zoneParametrs.Count - 1 : Mathf.Clamp(maxZone, 0, zoneParametrs.Count - 1);

        if (min > max)
        {
            int tmp = min;
            min = max;
            max = tmp;
        }

        Vector2 centerPos = CenterZones;
        Vector2 toPoint = positionOnScreen - centerPos;

        // Направление от центра к точке (если точка в центре — выбрать вправо)
        Vector2 dir;
        float mag = toPoint.magnitude;
        if (mag <= Mathf.Epsilon)
        {
            dir = Vector2.right;
        }
        else
        {
            dir = toPoint / mag;
        }

        // Выбираем ближайшую точку на разрешённых орбитах
        float bestDistSqr = float.MaxValue;
        Vector2 bestPos = centerPos;
        int bestZone = min;

        for (int z = min; z <= max; z++)
        {
            float radius = GetRadius(z);
            Vector2 candidate = centerPos + dir * radius;
            float dSqr = (candidate - positionOnScreen).sqrMagnitude;
            if (dSqr < bestDistSqr)
            {
                bestDistSqr = dSqr;
                bestPos = candidate;
                bestZone = z;
            }
        }

        // Вычисляем угловую позицию в градусах и нормализуем
        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angleDeg = PlanetTransform.PositionClamp(angleDeg);

        zone = bestZone;
        position = angleDeg;

        return new Vector3(bestPos.x, bestPos.y, 0f);
    }

    public float GetPerimetrZone(int zone, float offset)
    {
        return (GetRadius(zone) + offset) * 2 * Mathf.PI;
    }
}

[Serializable]
public class PlanetZoneParametrs
{
    public float distAtPreviousZone = 5f;
}