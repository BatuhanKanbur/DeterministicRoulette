using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AssetConstants
{
    private const string ChipObjectsLabelName = "ChipObject";
    public const string ChipWinParticle01 = "WinParticle_01";
    public const string ChipWinParticle02 = "WinParticle_02";
    public const string ChipLostParticle01 = "LoseParticle_01";
    public const string ChipDropParticle = "ChipDrop";
    public const string BallHistoryPrefab = "BallHistory";
    public static readonly AssetLabelReference ChipObjectsLabel = CreateLabelReference(ChipObjectsLabelName);
    private static AssetLabelReference CreateLabelReference(string label)
    {
        var labelRef = new AssetLabelReference {labelString = label};
        return labelRef;
    }
    public static string GetChipParticleName(int betMultiplier)
    {
        return betMultiplier <= 17 ? ChipWinParticle02 : ChipWinParticle01;
    }
}
