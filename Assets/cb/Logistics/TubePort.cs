using System.Collections.Generic;
using UnityEngine;

class TubePort : MonoBehaviour, ITubeReceiver, ITubeProvider
{
    readonly List<OutboundSchedule> _outbound = new List<OutboundSchedule>();

    public TubeBlockLink Inbound;
    public TubeBlockLink Outbound;
    public float Speed = 1;

    void Update()
    {
        var toRemove = new List<OutboundSchedule>();

        foreach (var outbound in _outbound)
        {
            if (outbound.Time <= Time.time)
            {
                Outbound.Destination.Receive(outbound.Color);
                toRemove.Add(outbound);
            }
        }

        foreach (var item in toRemove)
            _outbound.Remove(item);
    }

    public void Receive(BallColor color)
    {
        var time = Outbound.Length / Speed;
        _outbound.Add(new OutboundSchedule(color, Time.time + time));
    }

    public bool CanReceive(BallColor color)
    {
        return Outbound?.Destination != null &&
               Outbound.Destination.CanReceive(color);
    }
}