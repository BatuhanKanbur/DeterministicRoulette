using UnityEngine;

public interface IBetArea
{
    public void CheckBet(Transform betPoint,out bool isBetValid,out int betMultiplier,out int[] betNumbers);
}

