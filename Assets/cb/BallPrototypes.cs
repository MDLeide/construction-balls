using System;

[Serializable]
class BallPrototypes
{
    public Ball Blue;
    public Ball Red;
    public Ball Yellow;
    public Ball Green;
    public Ball Orange;
    public Ball Purple;

    public Ball GetPrototype(BallColor color)
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