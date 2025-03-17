using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HookBehaviour : MonoBehaviour
{
    [SerializeField] private float hookUpdateInterval = 1;
    private int _updateInterval;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly Dictionary<MemberInfo, object> _hookValues = new();

    private void OnEnable() => InitHookVars();

    private void OnDisable() => _cancellationTokenSource?.Cancel();

    private void InitHookVars()
    {
        _updateInterval = (int)(hookUpdateInterval * 1000);
        var members = GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var member in members)
        {
            if (Attribute.IsDefined(member, typeof(HookVar)))
            {
                switch (member)
                {
                    case FieldInfo field:
                        _hookValues[field] = field.GetValue(this);
                        break;
                    case PropertyInfo { CanRead: true } property:
                        _hookValues[property] = property.GetValue(this);
                        break;
                }
            }
        }
        _cancellationTokenSource = new CancellationTokenSource();
        CheckSyncVars(_cancellationTokenSource.Token).Forget();
    }

    private async UniTaskVoid CheckSyncVars(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(_updateInterval, cancellationToken: token);
                var members = GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var member in members)
                {
                    if (!Attribute.IsDefined(member, typeof(HookVar))) continue;
                    var currentValue = member switch
                    {
                        FieldInfo field => field.GetValue(this),
                        PropertyInfo { CanRead: true } property => property.GetValue(this),
                        _ => null
                    };
                    if (!_hookValues.ContainsKey(member) || _hookValues[member].Equals(currentValue)) continue;
                    var attr = (HookVar)Attribute.GetCustomAttribute(member, typeof(HookVar));
                    if (!string.IsNullOrEmpty(attr.HookMethod))
                    {
                        var eventInfo = GetType().GetRuntimeEvent(attr.HookMethod);
                        if (eventInfo != null)
                        {
                            var fieldInfo = GetType().GetField(attr.HookMethod, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                            var eventDelegate = fieldInfo?.GetValue(this) as Delegate;
                            eventDelegate?.DynamicInvoke(_hookValues[member], currentValue);
                        }
                        else
                        {
                            var methodInfo = GetType().GetMethod(attr.HookMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                            methodInfo?.Invoke(this, new object[] { _hookValues[member], currentValue });
                        }
                    }
                    _hookValues[member] = currentValue;
                }
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("CheckSyncVars Task Cancelled");
        }
    }
}
