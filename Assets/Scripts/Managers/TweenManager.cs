using System;
using System.Threading.Tasks;
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
    public static async void TweenPunch(this Transform targetObject,float scaleMultiplier = 1.25f,float duration=1f, bool autoClose=false)
    {
        var originalScale = targetObject.localScale;
        targetObject.localScale = Vector3.zero;
        targetObject.TweenScale(originalScale*scaleMultiplier,duration/2);
        await Task.Delay(TimeSpan.FromSeconds(duration));
        targetObject.TweenScale(originalScale,duration/2);
        await Task.Delay(TimeSpan.FromSeconds(duration));
        if (!autoClose) return;
        targetObject.TweenScale(Vector3.zero,duration/2);
        await Task.Delay(TimeSpan.FromSeconds(duration));
        targetObject.gameObject.SetActive(false);
        targetObject.localScale = originalScale;
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
