using System;

class BallCollectedEventArgs : EventArgs
{
    public BallCollectedEventArgs(BallColor color)
    {
        Color = color;
    }

    /// <summary>
    /// Set to true to indicate the receipt has been handled and the ball should be destroyed.
    /// </summary>
    public bool Handled { get; set; }
    public BallColor Color { get; }
}