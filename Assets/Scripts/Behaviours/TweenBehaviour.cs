using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TweenBehaviour : MonoBehaviour
{
    public IEnumerator MoveToTarget(Transform targetObject,Vector3 targetPosition, float duration, bool easeOut)
    {
        var isUI = targetObject.GetComponent<RectTransform>();
        var startPosition =isUI ? isUI.anchoredPosition3D : targetObject.position;
        var timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            var t = timeElapsed / duration;
            t = easeOut ? EaseOut(t) : EaseIn(t);
            if(isUI) isUI.anchoredPosition3D = Vector3.Lerp(startPosition, targetPosition, t);
            else targetObject.position = Vector3.Lerp(startPosition, targetPosition, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if(isUI) isUI.anchoredPosition3D = targetPosition;
        else targetObject.position = targetPosition;
    }
    public IEnumerator RotateToTarget(Transform targetObject,Vector3 targetRotation, float duration, bool easeOut)
    {
        var startRotation = targetObject.rotation;
        var endRotation = Quaternion.Euler(targetRotation);
        var timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            var t = timeElapsed / duration;
            t = easeOut ? EaseOut(t) : EaseIn(t);
            targetObject.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        targetObject.rotation = endRotation;
    }
    public IEnumerator ScaleToTarget(Transform targetObject,Vector3 targetScale, float duration, bool easeOut)
    {
        var startScale = targetObject.localScale;
        var timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            var t = timeElapsed / duration;
            t = easeOut ? EaseOut(t) : EaseIn(t);
            targetObject.localScale = Vector3.Lerp(startScale, targetScale, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        targetObject.localScale = targetScale;
    }
    public IEnumerator TweenToText(TextMeshProUGUI targetText,string newText, float duration)
    {
        if(IsNumeric(targetText.text) && IsNumeric(newText))
        {
            var startNumber = int.Parse(targetText.text);
            var endNumber = int.Parse(newText);
            StartCoroutine(LerpNumber(targetText, startNumber, endNumber, duration));
            yield return new WaitForSeconds(duration);
        }
        else
        {
            StartCoroutine(LerpText(targetText, newText, duration));
            yield return new WaitForSeconds(duration);
        }
    }
    private IEnumerator LerpText(TextMeshProUGUI targetText,string newText, float duration)
    {
        var timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            var t = timeElapsed / duration;
            var currentText=LerpTextAtIndex(newText, t);
            targetText.text = currentText;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        targetText.text = newText;
    }
    private IEnumerator LerpNumber(TextMeshProUGUI targetText,int startNumber, int endNumber, float duration)
    {
        var timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            var t = timeElapsed / duration;
            var currentNumber = Mathf.FloorToInt(Mathf.Lerp(startNumber, endNumber, t));
            targetText.text = currentNumber.ToString();
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        targetText.text = endNumber.ToString();
    }
    private bool IsNumeric(string str)
    {
        return double.TryParse(str, out _);
    }
    private string LerpTextAtIndex(string target, float t)
    {
        var targetLength = target.Length;
        var currentLength = Mathf.FloorToInt(targetLength * t);
        var result = "";
        for (var i = 0; i < currentLength; i++)
        {
            result += target[i];
        }
        return result;
    }
    private float EaseIn(float t)
    {
        return Mathf.Pow(t, 2);
    }
    private float EaseOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 2);
    }
}
