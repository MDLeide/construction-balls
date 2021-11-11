using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class Format
{
    public static string Number(int number)
    {
        return number.ToString();
    }

    public static string Number(float number)
    {
        return number.ToString();
    }

    public static string Percent(float number)
    {
        return number.ToString("N1");
    }

    public static string Distance(float distance)
    {
        string d;

        if (distance > 100)
            d = Math.Round(distance, 0, MidpointRounding.AwayFromZero).ToString();
        else if (distance > 10)
            d = Math.Round(distance, 1, MidpointRounding.AwayFromZero).ToString();
        else
            d = Math.Round(distance, 2, MidpointRounding.AwayFromZero).ToString();

        while (d.Contains('.') && d.Last() == '0')
            d = d.Substring(0, d.Length - 1);

        while (d.Last() == '.')
            d = d.Substring(0, d.Length - 1);

        return $"{d}m";
    }
}