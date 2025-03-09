using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class DIContainer
{
    private static readonly Dictionary<Type, object> Container = new Dictionary<Type, object>();
    public static void Register(Type type, object instance) => Container[type] = instance;
    public static T Resolve<T>() => (T)Container[typeof(T)];
    
    public static void AutoInject(MonoBehaviour target)
    {
        var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute(typeof(Inject)) == null) continue;
            if (Container.TryGetValue(field.FieldType, out var dependency))
                field.SetValue(target, dependency);
            else
                Debug.LogWarning(target.gameObject.name+" dependency not found for field: " + field.Name);
        }
    }
}
