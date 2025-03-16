using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class Bet : IDisposable
{
    public int[] BetNumbers{get;private set;}
    public int BetMultiplier{get;private set;}
    public int BetValue{get;private set;}
    public int TotalBetValue => BetValue * BetMultiplier;
    public GameObject BetChipObject{get;private set;}
    private AssetReferenceGameObject _chip3DAssetReferenceGameObject;
    public Vector3 BetPosition{get;private set;}
    public Vector3 BetRotation{get;private set;}
    public Bet(ChipObject chipObject,int[] betNumbers, int betMultiplier)
    {
        BetValue = chipObject.chipPrice;
        BetNumbers = betNumbers;
        BetMultiplier = betMultiplier;
        BetPosition = chipObject.Chip3DPrefab.transform.position;
        BetRotation = chipObject.Chip3DPrefab.transform.eulerAngles;
        _chip3DAssetReferenceGameObject = chipObject.chip3DPrefabAsset;
    }
    public async void InitChipObject()
    {
        if(BetChipObject) return;
        BetChipObject = await ObjectManager.GetObject(_chip3DAssetReferenceGameObject);
        _ = ObjectManager.GetObject(AssetConstants.ChipDropParticle,BetPosition);
        BetChipObject.gameObject.SetActive(true);
        BetChipObject.transform.position = BetPosition;
        BetChipObject.transform.eulerAngles = BetRotation;
    }
    public bool IsWinnable(int casinoBudget)
    {
        return casinoBudget >= TotalBetValue;
    }

    public void Dispose()
    {
        BetChipObject?.SetActive(false);
        BetChipObject = null;
    }
}
