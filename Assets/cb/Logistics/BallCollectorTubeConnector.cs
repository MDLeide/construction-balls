using UnityEngine;

class BallCollectorTubeConnector : MonoBehaviour
{
    public BallCollector BallCollector;
    public TubeOrigin TubeOrigin;

    void Start()
    {
        BallCollector.BallCollected += OnBallCollected;
    }

    void OnBallCollected(object sender, BallCollectedEventArgs e)
    {
        if (TubeOrigin.CanReceive(e.Color))
        {
            TubeOrigin.Receive(e.Color);
            e.Handled = true;
        }
    }
}