using System;
using System.Collections.Generic;
using UnityEngine;

class BallCollector : MonoBehaviour
{
    const float RefreshSeconds = 5;

    readonly List<Ball> _inside = new List<Ball>();
    float _nextRefresh;


    public event EventHandler<BallCollectedEventArgs> BallCollected;

    void Update()
    {
        if (_nextRefresh <= Time.time)
        {
            var toRemove = new List<Ball>();

            foreach (var ball in _inside)
                if (ReceiveBall(ball, false))
                    toRemove.Add(ball);

            foreach (var ball in toRemove)
                _inside.Remove(ball);

            _nextRefresh = Time.time + RefreshSeconds;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var ball = other.gameObject.GetComponent<Ball>();
        if (ball == null)
            ball = other.gameObject.GetComponentInParent<Ball>();
        if (ball == null)
            return;

        ReceiveBall(ball, true);
    }

    void OnTriggerExit(Collider other)
    {
        var ball = other.gameObject.GetComponent<Ball>();
        if (ball == null)
            ball = other.gameObject.GetComponentInParent<Ball>();
        if (ball == null)
            return;

        _inside.Remove(ball);
    }

    bool ReceiveBall(Ball ball, bool addToInsideList)
    {
        var args = new BallCollectedEventArgs(ball.Color);
        BallCollected?.Invoke(this, args);

        if (args.Handled)
        {
            Destroy(ball.gameObject);
            return true;
        }
        
        if (addToInsideList && !_inside.Contains(ball))
            _inside.Add(ball);

        return false;
    }
}