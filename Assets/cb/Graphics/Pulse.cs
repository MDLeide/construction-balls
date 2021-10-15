using System;
using DigitalRuby.Tween;
using UnityEngine;

class Pulse
{
    Vector3 _default;
    Vector3 _shrunk;

    public bool Pulsing;
    public Transform Transform;
    public float CycleTime = 1f;

    public void StartPulsing()
    {
        if (CycleTime <= 0)
            throw new InvalidOperationException();
        _default = Transform.localScale;
        _shrunk = _default * .5f;
        Pulsing = true;
        Shrink();
    }

    public void StopPulsing()
    {
        Pulsing = false;
    }

    void Shrink()
    {
        TweenFactory.Tween(
            new object(),
            Transform.localScale,
            _shrunk,
            CycleTime,
            TweenScaleFunctions.CubicEaseInOut,
            p => Transform.localScale = p.CurrentValue,
            p => Grow());
    }

    void Grow()
    {
        TweenFactory.Tween(
            new object(),
            Transform.localScale,
            _default,
            CycleTime,
            TweenScaleFunctions.CubicEaseInOut,
            p => Transform.localScale = p.CurrentValue,
            p =>
            {
                if (Pulsing)
                    Shrink();
            });
    }
}