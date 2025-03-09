using System;
using UnityEngine;

[Serializable]
public class InsideBetArea : BetArea,IBetArea
{
    public int betValue;
    public Transform[] betCorners;
    public Transform[] betSplits;
    public void CheckBet(Transform betPoint,out bool isBetValid, out int betMultiplier, out int[] betNumbers)
    {
        if(betPoint == BetCenter)
        {
            isBetValid = true;
            betMultiplier = BetMultiplier;
            betNumbers = new int[1];
            betNumbers[0] = betValue;
            return;
        }
        foreach (var betCorner in betCorners)
        {
            if (betPoint != betCorner) continue;
            isBetValid = true;
            betMultiplier = BetMultiplier/4;
            betNumbers = new int[1];
            betNumbers[0] = betValue;
            return;
        }
        foreach (var betSplit in betSplits)
        {
            if (betPoint != betSplit) continue;
            isBetValid = true;
            betMultiplier = BetMultiplier/2;
            betNumbers = new int[1];
            betNumbers[0] = betValue;
            return;
        }
        isBetValid = false;
        betMultiplier = 0;
        betNumbers = Array.Empty<int>();
    }
}
