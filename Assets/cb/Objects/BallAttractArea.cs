using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
class BallAttractArea : MonoBehaviour
{
    //readonly List<Ball> _balls = new List<Ball>();

    public float Force = 1;
    public BoxCollider BoxCollider;
    public int FrameFrequency = 5;

    void Reset()
    {
        Force = 1;
        BoxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (Time.frameCount % FrameFrequency == 0)
        {
            var colliders = Physics.OverlapBox(BoxCollider.center + transform.position, BoxCollider.size / 2);

            foreach (var collider in colliders)
            {
                var ball = collider.GetComponentAnywhere<Ball>();
                if (ball == null)
                    continue;

                var direction = (transform.position - ball.transform.position).normalized;
                ball.RB.AddForce(direction * Force);
            }
        }
    }
}