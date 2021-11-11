using System;
using System.Diagnostics;
using UnityEngine;

static class Compass
{
    [DebuggerStepThrough]
    public static CompassDirection VectorToCompassDirection(Vector3 direction)
    {
        if (direction == Vector3.forward)
            return CompassDirection.North;
        if (direction == Vector3.right)
            return CompassDirection.East;
        if (direction == Vector3.back)
            return CompassDirection.South;
        if (direction == Vector3.left)
            return CompassDirection.West;
        if (direction == Vector3.up)
            return CompassDirection.Up;
        if (direction == Vector3.down)
            return CompassDirection.Down;

        throw new ArgumentOutOfRangeException(nameof(direction), "Direction must be a cardinal vector. I made that up, but you know what it means.");
    }

    [DebuggerStepThrough]
    public static Vector3 CompassDirectionToVector(CompassDirection direction)
    {
        switch (direction)
        {
            case CompassDirection.North:
                return Vector3.forward;
            case CompassDirection.East:
                return Vector3.right;
            case CompassDirection.South:
                return Vector3.back;
            case CompassDirection.West:
                return Vector3.left;
            case CompassDirection.Up:
                return Vector3.up;
            case CompassDirection.Down:
                return Vector3.down;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    [DebuggerStepThrough]
    public static CompassDirection GetOpposingDirection(CompassDirection direction)
    {
        switch (direction)
        {
            case CompassDirection.North:
                return CompassDirection.South;
            case CompassDirection.East:
                return CompassDirection.West;
            case CompassDirection.South:
                return CompassDirection.North;
            case CompassDirection.West:
                return CompassDirection.East;
            case CompassDirection.Up:
                return CompassDirection.Down;
            case CompassDirection.Down:
                return CompassDirection.Up;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    [DebuggerStepThrough]
    public static CompassDirection RotateClockwise(CompassDirection direction)
    {
        switch (direction)
        {
            case CompassDirection.North:
                return CompassDirection.East;
            case CompassDirection.East:
                return CompassDirection.South;
            case CompassDirection.South:
                return CompassDirection.West;
            case CompassDirection.West:
                return CompassDirection.North;
            case CompassDirection.Up:
                return CompassDirection.Up;
            case CompassDirection.Down:
                return CompassDirection.Down;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    [DebuggerStepThrough]
    public static CompassDirection RotateCounterClockwise(CompassDirection direction)
    {
        switch (direction)
        {
            case CompassDirection.North:
                return CompassDirection.West;
            case CompassDirection.East:
                return CompassDirection.North;
            case CompassDirection.South:
                return CompassDirection.East;
            case CompassDirection.West:
                return CompassDirection.South;
            case CompassDirection.Up:
                return CompassDirection.Up;
            case CompassDirection.Down:
                return CompassDirection.Down;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}