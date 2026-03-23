using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameGlobalDataPreload", menuName = "SGames/BallGlobalData")]
public class BallGlobalData : GameGlobalDataPreload<BallGlobalData>
{
    /*[SerializeField] private ColorBallToBallData[] colorBallToBallDatas;
    [HideInInspector] public SpecificDictionary<ColorBall, ColorBallToBallData> ColorBallToBallDatas;

    [Space, Header("Settings")]
    public int StepPrepare = 2;

    public override void SetBehaviour()
    {
        base.SetBehaviour();

        // Словарь создаётся здесь, при первом обращении, funcCompareToFind сразу назначен
        ColorBallToBallDatas = new SpecificDictionary<ColorBall, ColorBallToBallData>(
            colorBallToBallDatas ?? Array.Empty<ColorBallToBallData>(),
            value => value.BallColor
        );
    }*/
}

[Serializable]
public class ColorBallToBallData
{
    public ColorBall BallColor;
    public BallGridObjectData Data;
}

[Serializable]
public class BallGridObjectData
{
    public Color ColorSprite;
}