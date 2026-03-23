using System;
using UnityEngine;
using UnityEngine.Events;

public class MyTagReactor2D : MonoBehaviour
{
    [SerializeField] private bool anyTag = true;
    [SerializeField] private string[] includeTags;

    public event Action<GameObject> OnTagEnter;
    public event Action<GameObject> OnTagExit;

    public UnityEvent<GameObject> OnTagEnterEv;
    public UnityEvent<GameObject> OnTagExitEv;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        TestCollisionEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TestCollisionExit(collision);
    }

    private void TestCollisionEnter(Collider2D collision)
    {
        TestCollision(collision, OnTagEnter, (go) => OnTagEnterEv.Invoke(go));
    }

    private void TestCollisionExit(Collider2D collision)
    {
        TestCollision(collision, OnTagExit, (go) => OnTagExitEv.Invoke(go));
    }

    private void TestCollision(Collider2D collision, params Action<GameObject>[] actions)
    {
        if (collision.transform.TryGetComponent(out MyTagsContainer tagContainer))
        {
            if ((anyTag && tagContainer.Any(includeTags)) || (tagContainer.All(includeTags)))
                foreach (var action in actions)
                    action?.Invoke(collision.gameObject);
        }
    }
}
