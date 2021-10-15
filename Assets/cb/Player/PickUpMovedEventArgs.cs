using System;

class PickUpMovedEventArgs : EventArgs
{
    public PickUpMovedEventArgs(PickUp pickUp)
    {
        PickUp = pickUp;
    }

    public PickUp PickUp { get; }
}