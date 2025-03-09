using System;
using UnityEngine;

public static class TweenManager
{
    private static TweenBehaviour _tweenBehaviour;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitTween()
    {
        var newTweenObject = new GameObject("TweenBehaviour");
        _tweenBehaviour = newTweenObject.AddComponent<TweenBehaviour>();
    }
    public static void TweenMove(this Transform targetObject,TweenDirection targetDirection,float targetValue,float duration=1f, bool easeOut = false)
    {
        var targetPosition = targetObject.position;
        switch (targetDirection)
        {
            case TweenDirection.X:
                targetPosition.x += targetValue;
                break;
            case TweenDirection.Y:
                targetPosition.y += targetValue;
                break;
            case TweenDirection.Z:
                targetPosition.z += targetValue;
                break;
        }
        TweenMove(targetObject, targetPosition, duration, easeOut);
    }
    public static void TweenMove(this Transform targetObject,Vector3 targetPosition,float duration=1f, bool easeOut = false)
    {
        _tweenBehaviour.StartCoroutine(_tweenBehaviour.MoveToTarget(targetObject,targetPosition, duration, easeOut));
    }
    public static void TweenRotate(this Transform targetObject,Vector3 targetRotation,float duration=1f, bool easeOut = false)
    {
        _tweenBehaviour.StartCoroutine(_tweenBehaviour.RotateToTarget(targetObject,targetRotation, duration, easeOut));
    }
    public static void TweenScale(this Transform targetObject,Vector3 targetRotation,float duration=1f, bool easeOut = false)
    {
        _tweenBehaviour.StartCoroutine(_tweenBehaviour.ScaleToTarget(targetObject,targetRotation, duration, easeOut));
    }
    public static void TweenText(this TMPro.TextMeshProUGUI targetText,string newText, float duration=1f)
    {
        _tweenBehaviour.StartCoroutine(_tweenBehaviour.TweenToText(targetText,newText, duration));
    }
}
