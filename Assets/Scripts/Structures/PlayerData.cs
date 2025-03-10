using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int totalMoney=25000;
    public int earnedMoney=0;
    public List<int> oldNumbers = new();
    public int rewardPoolMoney=1000000;
    public int casinoMoney=0;
    public void UpdateData(int newTotalMoney,int newEarnedMoney,List<int> newOldNumbers,int newRewardPoolMoney,int newCasinoMoney)
    {
        totalMoney = newTotalMoney;
        earnedMoney = newEarnedMoney;
        oldNumbers = newOldNumbers;
        rewardPoolMoney = newRewardPoolMoney;
        casinoMoney = newCasinoMoney;
        SaveLoadSystem.Save(this);
    }
}
