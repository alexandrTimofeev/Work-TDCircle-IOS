using UnityEngine;

public class RandomazerPosition : MonoBehaviour
{
    [Header("Позиция")]
    [SerializeField] private bool isChangePositionX;
    [SerializeField] private Vector2 minMaxX;

    [Space]
    [SerializeField] private bool isChangePositionY;
    [SerializeField] private Vector2 minMaxY;

    [Header("Поворот")]
    [SerializeField] private bool isChangeRotationZ;
    [SerializeField] private Vector2 minMaxRotationZ;

    [Header("Масштаб")]
    [SerializeField] private bool isChangeScale;
    [SerializeField] private bool uniformScale = true;
    [SerializeField] private Vector2 minMaxScaleX = new Vector2(1f, 1f);
    [SerializeField] private Vector2 minMaxScaleY = new Vector2(1f, 1f);

    [Space]
    [SerializeField] private bool randomazeOnStart = true;

    private void Start()
    {
        if (randomazeOnStart)
            RandomazePositionAndRotation();
    }

    public void RandomazePositionAndRotation()
    {
        // --- Позиция ---
        Vector3 newPosition = transform.position;

        if (isChangePositionX)
            newPosition.x = Random.Range(minMaxX.x, minMaxX.y);

        if (isChangePositionY)
            newPosition.y = Random.Range(minMaxY.x, minMaxY.y);

        transform.position = newPosition;

        // --- Поворот ---
        if (isChangeRotationZ)
        {
            Vector3 newRotation = transform.eulerAngles;
            newRotation.z = Random.Range(minMaxRotationZ.x, minMaxRotationZ.y);
            transform.rotation = Quaternion.Euler(newRotation);
        }

        // --- Масштаб ---
        if (isChangeScale)
        {
            Vector3 newScale = transform.localScale;

            if (uniformScale)
            {
                float randomScale = Random.Range(minMaxScaleX.x, minMaxScaleX.y);
                newScale = new Vector3(randomScale, randomScale, newScale.z);
            }
            else
            {
                newScale.x = Random.Range(minMaxScaleX.x, minMaxScaleX.y);
                newScale.y = Random.Range(minMaxScaleY.x, minMaxScaleY.y);
            }

            transform.localScale = newScale;
        }
    }
}