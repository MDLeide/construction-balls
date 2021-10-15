using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

class BallStorage : MonoBehaviour
{
    [Space]
    public int Blue;
    public int MaxBlue;

    [Space]
    public int Red;
    public int MaxRed;
    
    [Space]
    public int Yellow;
    public int MaxYellow;

    [Space]
    public int Green;
    public int MaxGreen;

    [Space]
    public int Purple;
    public int MaxPurple;

    [Space]
    public int Orange;
    public int MaxOrange;

    public int TotalBalls => Blue + Red + Yellow + Green + Purple + Orange;

    public List<BallColor> ColorsAccepted;

    public bool CanAdd(BallColor color)
    {
        if (!ColorsAccepted.Contains(color))
            return false;

        var max = GetMax(color);
        if (max < 0)
            return true;

        return GetTotal(color) < GetMax(color);
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

    public int GetTotal(BallColor color)
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

class BallInventory : MonoBehaviour
{
    public BallStorage InternalStorage;
    public List<BallStorage> AttachedStorage;

    public EventHandler<BallInventoryChangedEventArgs> BallReceived;

    public bool PreferInternalStorage = true;
    public bool LockInternalStorage;

    public int Blue => GetColorTotal(BallColor.Blue);
    public int Red => GetColorTotal(BallColor.Red);
    public int Yellow => GetColorTotal(BallColor.Yellow);

    public int Green => GetColorTotal(BallColor.Green);
    public int Orange => GetColorTotal(BallColor.Orange);
    public int Purple => GetColorTotal(BallColor.Purple);



    int GetColorTotal(BallColor color)
    {
        return InternalStorage.GetTotal(color) + AttachedStorage.Sum(p => p.GetTotal(color));
    }


    public bool CanPay(BallCost cost)
    {
        return cost.Blue <= Blue &&
               cost.Red <= Red &&
               cost.Yellow <= Yellow &&
               cost.Green <= Green &&
               cost.Purple <= Purple &&
               cost.Orange <= Orange;
    }

    public bool Pay(BallCost cost)
    {
        if (!CanPay(cost))
            return false;

        Blue -= cost.Blue;
        Red -= cost.Red;
        Yellow -= cost.Yellow;

        Green -= cost.Green;
        Purple -= cost.Purple;
        Orange -= cost.Orange;

        return true;
    }

    public bool Add(Ball ball)
    {
        if (TryAdd(ball.Color))
        {
            BallReceived?.Invoke(this, new BallInventoryChangedEventArgs(ball.Color, 1));
            return true;
        }

        return false;
    }

    bool TryAdd(BallColor color)
    {
        if (PreferInternalStorage && !LockInternalStorage)
        {
            if (!InternalStorage.Add(color))
                return AddToAttached(color);
            else
                return true;
        }
        else
        {
            if (AddToAttached(color))
                return true;

            if (!LockInternalStorage)
                return InternalStorage.Add(color);
            else
                return false;
        }
    }

    bool AddToAttached(BallColor color)
    {
        foreach (var ballStorage in AttachedStorage)
            if (ballStorage.Add(color))
                return true;

        return false;
    }
}