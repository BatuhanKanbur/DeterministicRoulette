using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameConstants
{
    public const string EncryptionKey = "Developed-By-BatuhanKanbur*09.03.2025";
    public const string BetTableEmptyText = 
        "<align=center>" +
        "\n<size=2><b>You haven't bet yet!</b></size>" +
        "</align>"
        ;
    public const float SpinDuration = 3f;
    public const float SpinSlowdownDuration = 1.5f;
    public const float SpinRotationMultiplier = 0.5f;
    public static readonly Vector4 BallNumberOfJumps = new Vector4(0, 360, 2, 7);
    public static readonly Vector4 BallJumpDuration = new Vector4(0, 360, 0.25f,1.25f);
    public static readonly Vector4 BallHeightFactor = new Vector4(0, 360, 0.15f, 0.5f);
    public static readonly Vector3 BallStartPosition = new Vector3(-0.9f, 0, 0);
    public static Vector3 BallStartRotation;
    public const float BallSpeed = 75f;
    public static readonly int[] RedNumbers = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
    public static readonly int[] BlackNumbers = { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 };
    public static Color GetColorFromNumber(int number)
    {
        if (number == 0)
        {
            return Color.green;
        }
        return RedNumbers.Contains(number) ? Color.red : Color.black;
    }
    public static ColorType GetColorTypeFromNumber(int number)
    {
        if (number == 0)
        {
            return ColorType.Green;
        }
        return RedNumbers.Contains(number) ? ColorType.Red : ColorType.Black;
    }
    public static string GetBetFrameText(int[] numbers,int multiplier)
    {
        var colorText = "";
        foreach (var number in numbers)
        {
            var newColor = ColorUtility.ToHtmlStringRGB(GetColorFromNumber(number));
            colorText +=$"<size=3><color=#{newColor}>{number}<sup>{multiplier}</sup></color></size> ";
        }
        return colorText;
    }
}
