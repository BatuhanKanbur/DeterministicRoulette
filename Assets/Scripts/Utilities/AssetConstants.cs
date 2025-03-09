using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AssetConstants
{
    private const string ChipObjectsLabelName = "ChipObject";
    public static readonly AssetLabelReference ChipObjectsLabel = CreateLabelReference(ChipObjectsLabelName);
    private static AssetLabelReference CreateLabelReference(string label)
    {
        var labelRef = new AssetLabelReference {labelString = label};
        return labelRef;
    }
}
