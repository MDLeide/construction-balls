using System;
using UnityEngine;

class PickUpPlacingEventArgs : EventArgs
{
    public PickUpPlacingEventArgs(Vector3 targetLocalPosition)
    {
        TargetLocalPosition = targetLocalPosition;
    }

    public Vector3 TargetLocalPosition;
}