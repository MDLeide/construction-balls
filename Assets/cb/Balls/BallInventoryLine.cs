using System;
using Sirenix.OdinInspector;

[Serializable]
class BallInventoryLine
{
    public BallInventoryLine() { }
    
    public BallInventoryLine(BallColor color, int max = -1)
    {
        Color = color;
        Max = max;
    }

    public BallColor Color;
    public int Max = -1;
    [ReadOnly]
    public int Total;

    public bool IsMaxed => Total >= Max && Max >= 0;

    public float PercentageOfMax
    {
        get
        {
            if (Max < 0)
                return 0;
            return Total / (float) Max;
        }
    }
}