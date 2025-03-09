using UnityEngine;

public class TransformConstants
{
    public const int IdleBarYTweenValue = 190;
    public static readonly LayerMask BetAreaLayerMask = 1 << 3;
    public static readonly Vector3 ChipHitRotation = new(0,180,0);
    public static readonly Vector3 ChipIdleRotation = new(-30,180,0);
    public static readonly Vector3 TopBarIdlePosition = new(0,0,0);
    public static readonly Vector3 TopBarBettingPosition = new(0,300,0);
    public static readonly Vector3 BottomBarIdlePosition = new(0,0,0);
    public static readonly Vector3 BottomBarBettingPosition = new(0,-300,0);
}
