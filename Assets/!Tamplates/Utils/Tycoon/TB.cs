using System.Collections;
using UnityEngine;

public static class TB
{

    private static string ClR => "white";
    private static string ClM => "green";

    public static string R => $"<Color={ClR}>R</Color>";
    public static string M => $"<Color={ClM}>$</Color>";
    public static string X => $"x";
    public static string PAdd => $"x+";
    public static string PUp => $"xx";
    public static string S => $"/s ";
    public static string Rs => $"{R}<Color={ClR}>{S}</Color>";
    public static string Ms => $"{M}<Color={ClM}>{S}</Color>";
    public static string Dots => "<size=4>...</size>";

    public static string PowerUp => "PowerUp";
    public static string SpeedUp => "SpeedUp";

}