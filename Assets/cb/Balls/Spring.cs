using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

class Spring : MonoBehaviour
{
    float _nextSpawn;

    [Header("Spawn")]
    public bool Randomize = true;
    public BallColor Color;
    public Transform SpawnLocation;
    [Space]
    public float Interval;
    
    [Header("Force")]
    public bool ApplyForce;
    public Vector3 ForceToApply;

    [ShowInInspector]
    public float BallsPerMinute => 60 / Interval;

    void Start()
    {
        _nextSpawn = Time.time;

        if (Randomize)
        {
            var val = Random.value;

            if (val <= .4f)
            {
                Color = BallColor.Blue;
            }
            else if (val <= .7f)
            {
                Color = BallColor.Red;
            }
            else
            {
                Color = BallColor.Yellow;
            }
        }
    }

    void Update()
    {
        if (_nextSpawn <= Time.time)
        {
            var ball = Instantiate(GetPrototype(), Game.Instance.BallContainer);
            ball.transform.position = SpawnLocation.position;
            _nextSpawn += Interval;

            if (ApplyForce)
                ball.RB.AddForce(ForceToApply);
        }
    }

    [Button]
    public void Snap()
    {
        var x = transform.position.x % 5;
        var z = transform.position.z % 5;

        x = transform.position.x - x;
        z = transform.position.z - z;

        transform.position = new Vector3(
            x + .25f,
            0,
            z + .25f);
    }

    Ball GetPrototype()
    {
        return Game.Instance.BallPrototypes.GetPrototype(Color);
    }
}