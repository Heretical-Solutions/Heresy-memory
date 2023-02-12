using System;

/*
using System.Globalization;
using UnityEngine;
*/

public static partial class StringExtensions
{
    /*
    public static Vector3 ToVector3(this string str)
    {
        str = str.Replace("(", "");
        str = str.Replace(")", "");
            
        var split = str.Split(',');
            
        return new Vector3(float.Parse(split[0], CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(split[1], CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(split[2], CultureInfo.InvariantCulture.NumberFormat));
    }
    */
    
    /// <summary>
    /// Return type.ToString() withour namespace
    /// </summary>
    /// <param name="type">Target type</param>
    /// <returns>Type name</returns>
    public static string ToBeautifulString(this Type type)
    {
        //LINQ
        //return type.ToString().Split('.').Last();

        var slices = type.ToString().Split('.');

        return slices[slices.Length - 1];
    }
}