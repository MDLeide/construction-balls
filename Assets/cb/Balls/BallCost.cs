using System;
using System.Collections.Generic;
using System.Linq;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine.UIElements;

[Serializable]
class BallCost
{
    public int Blue;
    public int Red;
    public int Yellow;
    public int Green;
    public int Purple;
    public int Orange;

    public int GetCost(BallColor color)
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