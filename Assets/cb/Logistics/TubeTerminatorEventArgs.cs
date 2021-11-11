using System;

class TubeTerminatorEventArgs : EventArgs
{
    public TubeTerminatorEventArgs(BallColor color)
    {
        Color = color;
    }

    public BallColor Color { get; }
    public bool Handled { get; set; }
}