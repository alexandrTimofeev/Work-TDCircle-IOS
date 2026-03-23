using UnityEngine;

public class VelectoryRotate : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Vector2 velocityYRange = new Vector2(-5f, 5f); // x = min, y = max

    [Space]
    [SerializeField] private AudioSource source; // x = min, y = max
    [SerializeField] private Vector2 velocityYRangePitch = new Vector2(-5f, 5f); // x = min, y =  max
    [SerializeField] private Vector2 velocityYRangeVoume = new Vector2(0.1f, 0.5f); // x = min, y = max

    private Rigidbody2D rigidbody2D;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (sprites == null || sprites.Length == 0 || renderer == null)
            return;

        float vy = rigidbody2D.linearVelocity.y;

        // Clamp vy между min и max
        float t = Mathf.InverseLerp(velocityYRange.x, velocityYRange.y, vy);

        // Переводим t (0..1) в индекс массива спрайтов
        int index = Mathf.Clamp(Mathf.RoundToInt(t * (sprites.Length - 1)), 0, sprites.Length - 1);

        renderer.sprite = sprites[sprites.Length - 1 - index];

        if (source)
        {
            source.pitch = Mathf.Lerp(velocityYRangePitch.x, velocityYRangePitch.y, t);
            float vx = rigidbody2D.linearVelocity.x;
            float tx = Mathf.InverseLerp(0, velocityYRange.y * 10, vx);
            source.volume = Mathf.Lerp(velocityYRangeVoume.x, velocityYRangeVoume.y, tx);
        }
    }
}