using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextDataUtils
{
    public static string BestScoreTxt { get => "BEST SCORE"; }
    public static string StatisticTxt { get => "STATISTIC"; }
    public static string TotalAttempsTxt { get => "Total attemps"; }
    public static string RecordTxt { get => "Record"; }
    public static string EasyTxt { get => "Easy"; }
    public static string NormalTxt { get => "Average"; }
    public static string HardTxt { get => "Hard"; }

    public static string RecordEasyTxt { get => $"{RecordTxt} {EasyTxt}"; }
    public static string RecordNormalTxt { get => $"{RecordTxt} {NormalTxt}"; }
    public static string RecordHardTxt { get => $"{RecordTxt} {HardTxt}"; }

}
