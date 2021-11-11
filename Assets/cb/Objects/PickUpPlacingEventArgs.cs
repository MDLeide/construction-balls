using System;
using UnityEngine;

class PickUpPlacingEventArgs : EventArgs
{
    public PickUpPlacingEventArgs(Vector3 targetPosition)
    {
        TargetPosition = targetPosition;
    }

    public Vector3 TargetPosition;
}