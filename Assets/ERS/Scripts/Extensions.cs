using System;
using System.ComponentModel;

public static class Extensions
{
    public static string GetDescription(this Enum val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
           .GetType()
           .GetField(val.ToString())
           .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : val.ToString();
    }

    public static T Next<T>(this T val) where T : Enum
    {
        T[] arr = (T[])Enum.GetValues(val.GetType());
        int i = Array.IndexOf(arr, val) + 1;
        return (i == arr.Length) ? arr[0] : arr[i];
    }

    public static T Previous<T>(this T val) where T : Enum
    {
        T[] arr = (T[])Enum.GetValues(val.GetType());
        int i = Array.IndexOf(arr, val) - 1;
        return (i == -1) ? arr[^1] : arr[i];
    }
}