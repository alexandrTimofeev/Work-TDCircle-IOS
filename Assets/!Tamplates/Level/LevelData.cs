using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "SGames/LevelData")]
public class LevelData : ScriptableObject
{
    public string ID;
    public int ScoreTarget = 100;
    public int Timer = 100;
    public GameObject LevelPrefab;
    public Sprite Background;

    [Space]
    public float DistFinalKof = 1f;
    public float StartWeghitCap = 1f;
    public float StartWeghitAllLineKof = 1f;

    [Space, Header("Presets")]
    public string[] BuildPresets;

    [Space]
    public SpawnPreset SpawnPreset;

    public static LevelData LoadFromResources (int level)
    {
        return Resources.Load<LevelData>($"Levels/LevelData{level}");
    }

    public BuildDataPreset[] GetPresets()
    {
        List<BuildDataPreset> buildDatas = new List<BuildDataPreset>();
        foreach (var presetName in BuildPresets)
        {
            buildDatas.Add(G.GlobalData.BuildPresets.Where((p) => p.ID == presetName).First());
        }

        return buildDatas.ToArray();
    }
}