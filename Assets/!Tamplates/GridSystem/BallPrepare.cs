using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class BallPrepare : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private ColorBallToBallData ballData;
    public ColorBall _ColorBall { get; private set; }

    private int count;
    private int maxCount;
    public int Count => count;
    public bool IsReady => count >= maxCount;

    public Vector2Int Position => position;

    private Vector2Int position;

    public Action<BallPrepare> OnComplite;

    private Tweener tweenerProgress;

    public void SetColorBall(ColorBall colorBall)
    {
        _ColorBall = colorBall;
        //ballData = BallGlobalData.FromResources.ColorBallToBallDatas[colorBall];

        spriteRenderer.color = ballData.Data.ColorSprite - new Color(0, 0, 0, 0.5f);
    }

    public void Init(int current, int max, Vector2Int position)
    {
        count = current;
        maxCount = max;
        SetPosition(position);
    }

    public IEnumerator SetPtogress (int current, int max)
    {
        if (tweenerProgress != null)
            tweenerProgress.Kill();

        tweenerProgress = transform.DOScale((current)/ (float)max, 0.5f);  
        yield return tweenerProgress.WaitForCompletion();   
    }

    public IEnumerator AddProgress()
    {
        count++;
        yield return SetPtogress(count, maxCount);
        if (count >= maxCount)
            OnComplite?.Invoke(this);
    }

    public void SetPosition (Vector2Int position)
    {
        this.position = position;
    }

    internal void Delite()
    {
        Destroy(gameObject);
    }
}
