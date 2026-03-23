using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;
using System;

public class GrapObject : MonoBehaviour
{
    public GrappableActionContainer Behaviour;
    [SerializeField] private List<string> targetTags = new List<string>();

    [Space]
    [SerializeField] private GameObject vfxGrap;

    [Space]
    [SerializeField] private GrapObjectAnimtionBehaviour animtionBehaviour;

    public UnityEvent OnGrap;

    public bool IsActive { get; protected set; } = true;

    public void Grap()
    {
        Delite();
        OnGrap?.Invoke();
    }

    public void Delite()
    {
        IsActive = false;

        if(animtionBehaviour.behaviourType != GrapObjectAnimBehType.None)
            StartCoroutine(DeliteCoroutine());
        else
            DeliteImmidiatly();
    }

    private IEnumerator DeliteCoroutine()
    {
        transform.DOKill(true);

        if(gameObject.TryGetComponent(out Collider2D collider2D))
            collider2D.enabled = false;

        if (gameObject.TryGetComponent(out Collider collider))
            collider.enabled = false;

        if (animtionBehaviour.behaviourType == GrapObjectAnimBehType.ScaleZero)
        {
            //Debug.Log($"DeliteCoroutine GrapObjectAnimBehType.ScaleZero");
            Tween tweenScale = transform.DOScale(0f, animtionBehaviour.duration);
            yield return new WaitUntil(() => tweenScale.IsComplete());
        }
        yield return null;
        DeliteImmidiatly();
    }

    protected virtual void DeliteImmidiatly()
    {
        Destroy(gameObject);
    }

    public bool ContainsAnyTags(string[] tags)
    {
        return targetTags.Any((s) => tags.Contains(s));
    }

    public GrappableActionContainer CloneAndGetBehaviour()
    {
        return Instantiate(Behaviour);
    }

    public void CreateVFX ()
    {
        if(vfxGrap != null)
            Instantiate(vfxGrap, transform.position, transform.rotation);
    }

    public void WorkAll()
    {       
        Behaviour.commands.WorkAll(GetContext());
    }

    public IEnumerator WorkAllRoutine()
    {
        GrappableObjectContext context = GetContext();        
        yield return Behaviour.commands.WorkAllRoutine(context);
    }

    public GrappableObjectContext GetContext()
    {
        return new GrappableObjectContext()
        {
            GrapObject = this,
        };
    }
}

public enum GrapObjectAnimBehType { None, ScaleZero }
[System.Serializable]
public class GrapObjectAnimtionBehaviour
{
    public GrapObjectAnimBehType behaviourType;
    public float duration = 1f;
}