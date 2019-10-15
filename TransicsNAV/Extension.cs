using System;

public static class ObjectExtensions
{
    public static string NullSafeToString(this object obj)
    {
        return obj != null ? obj.ToString() : String.Empty;
    }
    public static double? NullSafeToDouble(this double? obj)
    {
        return obj != null ? obj : 0.0;
    }
    public static object ToString(this object obj)
    {
        if (obj == null)
        {
            return "null";
        }
        else
        {
            return obj.GetType().Name;
        }
    }
}

