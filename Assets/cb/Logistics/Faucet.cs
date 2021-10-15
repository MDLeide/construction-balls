using UnityEditor.Experimental.GraphView;
using UnityEngine;

class Faucet : MonoBehaviour
{
    float _nextSpawn;

    public Ball Ball;
    public bool On;
    public float BallsPerMinute = 10;
    public float SecondsPerBall => 1 / (BallsPerMinute / 60);
    public Transform SpawnPoint;
    
    void Update()
    {
        if (_nextSpawn <= Time.time && On)
        {
            if (SpawnPoint != null)
                Instantiate(Ball, SpawnPoint.position, Quaternion.identity);
            else
                Instantiate(Ball, transform.position, Quaternion.identity);
            _nextSpawn = Time.time + SecondsPerBall;

            //_nextSpawn = 1 / (BallsPerMinute / 60);
        }
    }
}