using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

class TubeTerminator : MonoBehaviour, ITubeReceiver
{
    public event EventHandler<TubeTerminatorEventArgs> BallReceived;

    [Tooltip("If true, a ball will be spawned at the terminator's location if nothing is connected to it.")]
    public bool SpawnIfNotHandled;
    [Tooltip("This function defines whether or not the terminator can receive balls.")]
    public Func<BallColor, bool> CanReceiveFunc;

    public TubePort Port;

    public void Receive(BallColor color)
    {
        var args = new TubeTerminatorEventArgs(color);
        BallReceived?.Invoke(this, args);

        if (SpawnIfNotHandled && !args.Handled)
        {
            Ball.Spawn(color, transform.position);
        }
    }

    public bool CanReceive(BallColor color)
    {
        return CanReceiveFunc == null || CanReceiveFunc(color);
    }
}