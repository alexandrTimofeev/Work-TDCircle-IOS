using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BuildbleObject : MonoBehaviour
{
    [SerializeField] private PlanetTransform planetTransform;

    public UnityEvent OnBuild;

    public void OnBuildInit(int zone, float position, Vector2 worldPos, PlanetPositor planet)
    {
        if(planet)
        {
            planetTransform.Zone = zone;
            planetTransform.Position = position;
        }
        else
        {
            transform.position = worldPos;
        }

        OnBuild?.Invoke();
    }
}