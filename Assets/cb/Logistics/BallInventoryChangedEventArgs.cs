using System;

class BallInventoryChangedEventArgs : EventArgs
{
    public BallInventoryChangedEventArgs(BallColor color, int change)
    {
        Color = color;
        Change = change;
    }

    public BallColor Color { get; }
    public int Change { get; }
}