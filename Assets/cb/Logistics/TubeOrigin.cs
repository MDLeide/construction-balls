using System.Collections.Generic;
using UnityEngine;

class TubeOrigin : MonoBehaviour, ITubeProvider, ITubeReceiver
{
    readonly List<OutboundSchedule> _outbound = new List<OutboundSchedule>();

    public TubePort Port;
    public float Delay;
    
    void Update()
    {
        var toRemove = new List<OutboundSchedule>();

        foreach (var outbound in _outbound)
        {
            if (outbound.Time <= Time.time)
            {
                Port.Receive(outbound.Color);
                toRemove.Add(outbound);
            }
        }

        foreach (var item in toRemove)
            _outbound.Remove(item);
    }

    public void Receive(BallColor color)
    {
        _outbound.Add(new OutboundSchedule(color, Time.time + Delay));
    }

    public bool CanReceive(BallColor color)
    {
        return Port.CanReceive(color);
    }


    void Reset()
    {
        Port = GetComponent<TubePort>();
        if (Port == null)
            Port = gameObject.AddComponent<TubePort>();
    }
}