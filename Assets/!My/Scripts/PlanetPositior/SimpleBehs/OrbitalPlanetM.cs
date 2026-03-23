using System.Collections;
using UnityEngine;

public class OrbitalPlanetM : MonoBehaviour
{
    public float SpeedRotate = 1f;
    public float SpeedOffset = 0f;
    [SerializeField] private bool speedInDistance = false;

    private PlanetTransform planetTr;

    private void Start()
    {
        planetTr = GetComponent<PlanetTransform>();  
    }

    private void Update()
    {
        if(speedInDistance)
            planetTr.AddPositionInDistance(SpeedRotate * Time.deltaTime);
        else
            planetTr.Position += SpeedRotate * Time.deltaTime;
        planetTr.Offset += SpeedOffset * Time.deltaTime;
    }
}