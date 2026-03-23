using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class GrapCollider : MonoBehaviour
{
    [SerializeField] private List<string> grapTargetTags = new List<string>();
    [SerializeField] private GameObject vfxGrap;
    [SerializeField] private bool useStaticMediator = true;

    public static GrappableObjectMediator Mediator;

    public event Action<GrapObject> OnGrap;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out GrapObject grapObject) && grapObject.IsActive)
        {
            if (grapObject.ContainsAnyTags(grapTargetTags.ToArray()))
            {
                grapObject.Grap();
                Grap(grapObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GrapObject grapObject) && grapObject.IsActive)
        {
            if (grapObject.ContainsAnyTags(grapTargetTags.ToArray()))
            {
                grapObject.Grap();
                Grap(grapObject);
            }
        }
    }

    private void Grap(GrapObject grapObject)
    {
        grapObject.CreateVFX();
        if (vfxGrap != null)
            Instantiate(vfxGrap, transform.position, transform.rotation);
        if (useStaticMediator)
            Mediator.Invoke(grapObject);

        grapObject.WorkAll();

        OnGrap?.Invoke(grapObject);
    }
}