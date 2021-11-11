using System;
using System.Collections.Generic;
using UnityEngine;

class MixedColorCanister : MonoBehaviour
{
    [Space]
    public int Blue;
    public int MaxBlue = -1;

    [Space]
    public int Red;
    public int MaxRed = -1;

    [Space]
    public int Yellow;
    public int MaxYellow = -1;

    [Space]
    public int Green;
    public int MaxGreen = -1;

    [Space]
    public int Purple;
    public int MaxPurple = -1;

    [Space]
    public int Orange;
    public int MaxOrange = -1;

    public int TotalBalls => Blue + Red + Yellow + Green + Purple + Orange;

    public List<BallColor> ColorsAccepted = new List<BallColor>
    {
        BallColor.Blue,
        BallColor.Red,
        BallColor.Yellow,
        BallColor.Green,
        BallColor.Orange,
        BallColor.Purple
    };

    /// <summary>
    /// Returns [amount] - [paid]. So, if the full amount was paid, this will return 0. Otherwise,
    /// it will pay as much as it can and return whatever is left to be paid.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int Pay(BallColor color, int amount)
    {
        var total = GetQuantity(color);
        if (total >= amount)
        {
            PayColor(color, amount);
            return 0;
        }

        var remainder = amount - total;
        PayColor(color, total);
        return remainder;
    }

    void PayColor(BallColor color, int amount)
    {
        switch (color)
        {
            case BallColor.Blue:
                Blue -= amount;
                break;
            case BallColor.Red:
                Red -= amount;
                break;
            case BallColor.Yellow:
                Yellow -= amount;
                break;
            case BallColor.Green:
                Green -= amount;
                break;
            case BallColor.Purple:
                Purple -= amount;
                break;
            case BallColor.Orange:
                Orange -= amount;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }

    public bool CanAdd(BallColor color)
    {
        if (!ColorsAccepted.Contains(color))
            return false;

        var max = GetMax(color);
        if (max < 0)
            return true;

        return GetQuantity(color) < GetMax(color);
    }

    public bool Add(BallColor color)
    {
        if (!CanAdd(color))
            return false;

        switch (color)
        {
            case BallColor.Blue:
                Blue++;
                break;
            case BallColor.Red:
                Red++;
                break;
            case BallColor.Yellow:
                Yellow++;
                break;
            case BallColor.Green:
                Green++;
                break;
            case BallColor.Purple:
                Purple++;
                break;
            case BallColor.Orange:
                Orange++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    public int GetMax(BallColor color)
    {
        switch (color)
        {
            case BallColor.Blue:
                return MaxBlue;
            case BallColor.Red:
                return MaxRed;
            case BallColor.Yellow:
                return MaxYellow;
            case BallColor.Green:
                return MaxGreen;
            case BallColor.Purple:
                return MaxPurple;
            case BallColor.Orange:
                return MaxOrange;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }

    public int GetQuantity(BallColor color)
    {
        switch (color)
        {
            case BallColor.Blue:
                return Blue;
            case BallColor.Red:
                return Red;
            case BallColor.Yellow:
                return Yellow;
            case BallColor.Green:
                return Green;
            case BallColor.Purple:
                return Purple;
            case BallColor.Orange:
                return Orange;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }
}