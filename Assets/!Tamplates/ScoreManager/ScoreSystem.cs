using System;
using UnityEngine;

public class ScoreSystem
{
    public int Score { get; private set; }

    public event Action<ChangeScoreInfo> OnScoreChange;

    public ScoreSystem(int score = 0)
    {
        SetScore(score);
    }

    #region PUBLIC API

    public void AddScore(int value, Vector3? point = null)
    {
        Change(value, point);
    }

    public void RemoveScore(int value, Vector3? point = null)
    {
        Change(-value, point);
    }

    public void SetScore(int value, Vector3? point = null)
    {
        int delta = value - Score;

        ApplyChange(new ChangeScoreInfo
        {
            Value = value,
            Point = point,
            Delta = delta,
            ChangeType = ChangeScoreType.Set
        });
    }

    #endregion

    #region CORE

    private void Change(int delta, Vector3? point)
    {
        if (delta == 0)
            return;

        ApplyChange(new ChangeScoreInfo
        {
            Value = Score + delta,
            Point = point,
            Delta = delta,
            ChangeType = delta > 0
                ? ChangeScoreType.Add
                : ChangeScoreType.Remove
        });
    }

    private void ApplyChange(ChangeScoreInfo info)
    {
        Score = info.Value;
        OnScoreChange?.Invoke(info);
    }

    #endregion

    #region OPERATORS_INT_COMPARE

    public static ScoreSystem operator +(ScoreSystem score, int value)
    {
        score.AddScore(value);
        return score;
    }

    public static ScoreSystem operator -(ScoreSystem score, int value)
    {
        score.RemoveScore(value);
        return score;
    }

    public static bool operator >(ScoreSystem score, int value)
    {
        return score.Score > value;
    }

    public static bool operator <(ScoreSystem score, int value)
    {
        return score.Score < value;
    }

    public static bool operator >=(ScoreSystem score, int value)
    {
        return score.Score >= value;
    }

    public static bool operator <=(ScoreSystem score, int value)
    {
        return score.Score <= value;
    }

    public static bool operator ==(ScoreSystem score, int value)
    {
        if (score is null) return false;
        return score.Score == value;
    }

    public static bool operator !=(ScoreSystem score, int value)
    {
        if (score is null) return true;
        return score.Score != value;
    }

    public override bool Equals(object obj)
    {
        if (obj is ScoreSystem other)
            return Score == other.Score;

        return false;
    }

    public override int GetHashCode()
    {
        return Score.GetHashCode();
    }

    #endregion
}

public enum ChangeScoreType
{
    Set,
    Add,
    Remove
}

[Serializable]
public class ChangeScoreInfo
{
    public int Value;
    public Vector3? Point;
    public string Prefix;
    public string Postfix;

    [Space]
    public int Delta;
    public ChangeScoreType ChangeType;

    public override string ToString()
    {
        return $"{ChangeType}: {Delta} → {Value}";
    }
}