using System;
using UnityEngine;

public static class ExtendedMathf
{
    private static double RemapValue(double value, double fMin, double fMax, double tMin, double tMax, bool invert)
    {
        if (invert) (tMin, tMax) = (tMax, tMin);
        return tMin + (value - fMin) * (tMax - tMin) / (fMax - fMin);
    }

    public static T Remap<T>(this T value, Vector4 vectorValue, bool invert = false) where T : struct, IConvertible
    {
        var val = Convert.ToDouble(value);
        var remapped = RemapValue(val, Convert.ToDouble(vectorValue.x), Convert.ToDouble(vectorValue.y), 
                                  Convert.ToDouble(vectorValue.z), Convert.ToDouble(vectorValue.w), invert);
        return (T)Convert.ChangeType(remapped, typeof(T));
    }

    public static T RemapClamped<T>(this T value, Vector4 vectorValue, bool invert = false) where T : struct, IConvertible
    {
        var val = Convert.ToDouble(value);
        var remapped = RemapValue(val, Convert.ToDouble(vectorValue.x), Convert.ToDouble(vectorValue.y), 
                                  Convert.ToDouble(vectorValue.z), Convert.ToDouble(vectorValue.w), invert);
        remapped = Math.Clamp(remapped, Math.Min(Convert.ToDouble(vectorValue.z), Convert.ToDouble(vectorValue.w)), 
                                  Math.Max(Convert.ToDouble(vectorValue.z), Convert.ToDouble(vectorValue.w)));
        return (T)Convert.ChangeType(remapped, typeof(T));
    }

    public static T Remap<T>(this T value, T fromMin, T fromMax, T toMin, T toMax, bool invert = false) where T : struct, IConvertible
    {
        var val = Convert.ToDouble(value);
        var remapped = RemapValue(val, Convert.ToDouble(fromMin), Convert.ToDouble(fromMax), 
                                  Convert.ToDouble(toMin), Convert.ToDouble(toMax), invert);
        return (T)Convert.ChangeType(remapped, typeof(T));
    }

    public static T RemapClamped<T>(this T value, T fromMin, T fromMax, T toMin, T toMax, bool invert = false) where T : struct, IConvertible
    {
        var val = Convert.ToDouble(value);
        var remapped = RemapValue(val, Convert.ToDouble(fromMin), Convert.ToDouble(fromMax), 
                                  Convert.ToDouble(toMin), Convert.ToDouble(toMax), invert);
        remapped = Math.Clamp(remapped, Math.Min(Convert.ToDouble(toMin), Convert.ToDouble(toMax)), 
                                  Math.Max(Convert.ToDouble(toMin), Convert.ToDouble(toMax)));
        return (T)Convert.ChangeType(remapped, typeof(T));
    }
}
