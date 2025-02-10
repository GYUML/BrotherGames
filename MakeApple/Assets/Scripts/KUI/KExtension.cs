using UnityEngine;

public static class KExtension
{
    public static string ToFormat(this long number)
    {
        return number.ToString("#,##0");
    }

    public static string ToFormat(this int number)
    {
        return number.ToString("#,##0");
    }
}
