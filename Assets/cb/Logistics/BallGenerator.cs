using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class BallGenerator : MonoBehaviour
{
    float _next;

    public Vector3 ForceToApply = Vector3.forward;
    public Rigidbody BallPrototype;
    public PBlock InternalPipeBlock;
    public float BallDelay = 2f;

    void Reset()
    {
        InternalPipeBlock = gameObject.GetComponentInChildren<PBlock>();
    }

    void Update()
    {
        if (_next <= Time.time)
        {
            _next = Time.time + BallDelay;
            if (!InternalPipeBlock.IsConnected)
                return;

            var ball = Instantiate(BallPrototype);
            ball.transform.position = transform.position;
            ball.AddForce(transform.TransformDirection(ForceToApply));
        }
    }
}