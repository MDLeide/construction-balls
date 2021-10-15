using System;
using UnityEngine;

class InteractablePushedEventArgs : EventArgs
{
    public InteractablePushedEventArgs(RaycastHit? hit)
    {
        Hit = hit;
    }

    public RaycastHit? Hit { get; }
}