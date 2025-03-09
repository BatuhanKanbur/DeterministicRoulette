using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class HookBehaviour : MonoBehaviour
{
    [SerializeField] private float hookUpdateInterval = 1;
    private int _updateInterval;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly Dictionary<MemberInfo, object> _hookValues = new();

    private void OnEnable() => InitHookVars();

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
                    case PropertyInfo {CanRead: true} property:
                        _hookValues[property] = property.GetValue(this);
                        break;
                }
            }
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _ = CheckSyncVars(_cancellationTokenSource.Token);
    }

    private async Task CheckSyncVars(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(_updateInterval, token);
                var members = GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var member in members)
                {
                    if (!Attribute.IsDefined(member, typeof(HookVar))) continue;
                    var currentValue = member switch
                    {
                        FieldInfo field => field.GetValue(this),
                        PropertyInfo {CanRead: true} property => property.GetValue(this),
                        _ => null
                    };
                    if (!_hookValues.ContainsKey(member) || Equals(_hookValues[member], currentValue)) continue;
                    var attr = (HookVar)Attribute.GetCustomAttribute(member, typeof(HookVar));
                    if (!string.IsNullOrEmpty(attr.HookMethod))
                    {
                        var method = GetType().GetMethod(attr.HookMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        method?.Invoke(this, new object[] { _hookValues[member], currentValue });
                    }
                    _hookValues[member] = currentValue;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}