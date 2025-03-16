using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int totalMoney=25000;
    public int earnedMoney=0;
    public List<int> oldNumbers = new();
    public void UpdateData(int newTotalMoney,int newEarnedMoney,List<int> newOldNumbers)
    {
        totalMoney = newTotalMoney;
        earnedMoney = newEarnedMoney;
        oldNumbers = newOldNumbers;
        SaveLoadSystem.Save(this);
    }
}
