using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CheatUI : MonoBehaviour,ICheatUI
{
    [HookVar(nameof(OnValueChanged))] private float Value { get; set; }
    [SerializeField] private Text text;
    [SerializeField] private float minValue, maxValue, additionalValue;
    [SerializeField] private CheatUIType cheatUIType;
    public event Action<float,float> OnValueChanged;

    private void Start() => OnValueChanged += ShowText;
    private void OnDestroy() => OnValueChanged -= ShowText;
    private void ShowText(float oldValue,float newValue)
    {
        switch (cheatUIType)
        {
            case CheatUIType.Percent:
                text.text = $"%{newValue:F2}";
                break;
            case CheatUIType.Money:
                text.text = $"{newValue:N0}$";
                break;
            case CheatUIType.Value:
                text.text = newValue < 0 ? "RANDOM" : $"{newValue}";
                break;
        }
    }

    public void SetValue(float newValue) => Value = newValue;
    public void AddValue()
    {
        Value += additionalValue;
        if(Value > maxValue)
            Value = minValue;
    }

    public void RemoveValue()
    {
        Value -= additionalValue;
        if(Value < minValue)
            Value = maxValue;
    }
}
