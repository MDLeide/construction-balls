using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class ForceArea : MonoBehaviour
{
    readonly List<Ball> _balls = new List<Ball>();

    public float Power;

    void Update()
    {
        var toRemove = new List<Ball>();

        foreach (var ball in _balls)
        {
            if (ball == null || ball.RB == null)
                toRemove.Add(ball);
            else
                ball.RB.AddForce(transform.forward * Power);
        }

        foreach (var ball in toRemove)
            _balls.Remove(ball);
    }

    void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<Ball>();
        if (ball == null)
            ball = other.GetComponentInParent<Ball>();

        if (ball != null)
            _balls.Add(ball);
    }

    void OnTriggerExit(Collider other)
    {
        var ball = other.GetComponent<Ball>();
        if (ball == null)
            ball = other.GetComponentInParent<Ball>();

        if (ball != null)
            _balls.Remove(ball);
    }
}