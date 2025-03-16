using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BetManager : MonoBehaviour,IManager
{
    [Inject] private CheatManager _cheatManager;
    public BetParameterData BetParameterData { get; private set; }

    private void Start()
    {
        DIContainer.AutoInject(this);
        BetParameterData = SaveLoadSystem.Load<BetParameterData>();
    }

    public int GetSpinResult(List<Bet> playerBets)
    {
        var totalBet = playerBets.Sum(bet => bet.TotalBetValue);
        var maxPayout = BetParameterData.rewardPoolMoney - (int) (totalBet * (BetParameterData.returnToPlayer / 100f));
        var luckyNumbers = Enumerable.Range(0, 37).ToList();
        foreach (var playerBet in playerBets)
        {
            if (!playerBet.IsWinnable(maxPayout))
                luckyNumbers.RemoveAll(x => playerBet.BetNumbers.Contains(x));
            else
                maxPayout -= playerBet.TotalBetValue;
        }

        var totalProbability = BetParameterData.greenProbability + BetParameterData.redProbability + BetParameterData.blackProbability;
        var randomValue = UnityEngine.Random.Range(0f, totalProbability);
        if (randomValue > BetParameterData.redProbability + BetParameterData.blackProbability)
            luckyNumbers.Remove(0);
        if (randomValue > BetParameterData.blackProbability &&
            randomValue < BetParameterData.redProbability + BetParameterData.blackProbability)
            luckyNumbers.RemoveAll(x => GameConstants.BlackNumbers.Contains(x));
        else
            luckyNumbers.RemoveAll(x => GameConstants.RedNumbers.Contains(x));
        var spinResult = BetParameterData.targetSpin == -1
            ? luckyNumbers[UnityEngine.Random.Range(0, luckyNumbers.Count)]
            : BetParameterData.targetSpin;
        var lastCasinoBudget = BetParameterData.casinoBudget;
        var lastRewardPoolMoney = BetParameterData.rewardPoolMoney;
        foreach (var playerBet in playerBets)
        {
            if (playerBet.BetNumbers.Contains(spinResult))
            {
                lastRewardPoolMoney -= playerBet.BetValue;
            }
            else
            {
                lastRewardPoolMoney +=(int) ExtendedMathf.GetPercentage(playerBet.BetValue, BetParameterData.returnToPlayer);
                lastCasinoBudget += (int) ExtendedMathf.GetPercentage(playerBet.BetValue, BetParameterData.returnToPlayer,true);
            }
        }
        SetCasinoBudget(lastCasinoBudget);
        SetRewardPoolMoney(lastRewardPoolMoney);
        SaveLoadSystem.Save(BetParameterData);
        return spinResult;
    }

    public void SaveBetParameterData()
    {
        SaveLoadSystem.Save(BetParameterData);
    }
    public void ResetBetParameterData()
    {
        BetParameterData = new BetParameterData();
        SaveLoadSystem.Save(BetParameterData);
    }
    public void SetCasinoBudget(float value) => BetParameterData.casinoBudget = (int)value;
    public void SetRewardPoolMoney(float value) =>  BetParameterData.rewardPoolMoney = (int)value;
    public void SetTargetSpin(float value) => BetParameterData.targetSpin = (int)value;
    public void SetGreenProbability(float value) =>  BetParameterData.greenProbability = value;
    public void SetRedProbability(float value) => BetParameterData.redProbability = value;
    public void SetBlackProbability(float value) => BetParameterData.blackProbability = value;
    public void SetReturnToPlayer(float value) => BetParameterData.returnToPlayer = value;
}
