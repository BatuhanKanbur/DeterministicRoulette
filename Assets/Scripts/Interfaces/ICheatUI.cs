using System;
using UnityEngine;

public interface ICheatUI
{
    public event Action<float,float> OnValueChanged;
    public void SetValue(float value);
    public void AddValue();
    public void RemoveValue();
}
