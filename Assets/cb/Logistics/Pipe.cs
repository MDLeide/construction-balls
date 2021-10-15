using Cashew.Utility.Async;
using DigitalRuby.Tween;
using UnityEngine;

class Pipe : MonoBehaviour
{
    const float ForceMultiplier = 100;

    public Transform Exit;
    public float SpawnForce;
    public float TravelTime;

    void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        var ball = other.GetComponent<Ball>();
        if (ball == null)
            ball = other.GetComponentInParent<Ball>();

        if (ball == null)
            return;

        Transfer(ball);
    }

    void Transfer(Ball ball)
    {
        ball.GhostMode();
        ball.RB.MovePosition(Exit.position);
        ball.RB.velocity = Vector3.zero;

        _ = Execute.Later(
            TravelTime,
            () =>
            {
                ball.TurnOffGhostMode(); 
                ball.RB.AddForce(Exit.forward * SpawnForce * ForceMultiplier);
            });
    }
}