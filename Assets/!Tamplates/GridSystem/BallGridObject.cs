using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorBall { None, Red, Green, Blue }
public class BallGridObject : GridObjectBase, IGridObject
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private ColorBallToBallData ballData;

    [Space]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioDataPlay moveDataPlay;
    [SerializeField] private GameObject vfxCalculate;
    [SerializeField] private GameObject vfxDelite;

    public ColorBall _ColorBall { get; private set; }

    public override void Awake()
    {
        autoFindOnStart = false;
        Init(GameG.CellPlacer);

        base.Awake();
    }

    public void SetColorBall (ColorBall colorBall)
    {
        _ColorBall = colorBall;
        //ballData = BallGlobalData.FromResources.ColorBallToBallDatas[colorBall];

        spriteRenderer.color = ballData.Data.ColorSprite;
    }

    public override void CalculateWork(GridObjectCalcualteData calcualteData)
    {
        base.CalculateWork(calcualteData);

        GameG.ScoreSys.AddScore(100 * (calcualteData.gridObjectBasesPrev.Count + 1), transform.position);

        Destroy(Instantiate(vfxCalculate, transform.position, transform.rotation).gameObject, 10f);
    }

    public override void Delite()
    {
        Destroy(Instantiate(vfxDelite, transform.position, transform.rotation).gameObject, 10f);
        base.Delite();
    }

    public override IEnumerator MoveAnimationProcess(Vector2Int cellStart, Vector2Int cellEnd)
    {
        audioSource.Play(moveDataPlay);
        return base.MoveAnimationProcess(cellStart, cellEnd);
    }
}