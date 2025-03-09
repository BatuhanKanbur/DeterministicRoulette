using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class OutsideBetArea : BetArea, IBetArea
{
    public int minValue;
    public int maxValue;
    public BetType betType;
    public void CheckBet(Transform betPoint,out bool isBetValid, out int betMultiplier, out int[] betNumbers)
    {
        if(betPoint == BetCenter)
        {
            isBetValid = true;
            betMultiplier = BetMultiplier;
            betNumbers = GetBetNumbers();
            return;
        }
        isBetValid = false;
        betMultiplier = 0;
        betNumbers = new int[] { };
    }
    private int[] GetBetNumbers()
    {
        var numbers = new List<int>();
        switch (betType)
        {
            case BetType.Normal:
                for (var i = minValue; i <= maxValue; i += 1)
                    numbers.Add(i);
                break;
            case BetType.LowToHigh:
                for (var i = minValue; i <= maxValue; i += 3)
                    numbers.Add(i);
                break;
            case BetType.Red:
                numbers = GameConstants.RedNumbers.ToList();
                break;
            case BetType.Black:
                numbers = GameConstants.BlackNumbers.ToList();
                break;
            case BetType.Even:
                for (var i = minValue; i <= maxValue; i += 1)
                    if(i % 2 == 0)
                        numbers.Add(i);
                break;
            case BetType.Odd:
                for (var i = minValue; i <= maxValue; i += 1)
                    if(i % 2 != 0)
                        numbers.Add(i);
                break;
        }
        return numbers.ToArray();
    }
}
