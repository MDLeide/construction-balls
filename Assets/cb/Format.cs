using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}