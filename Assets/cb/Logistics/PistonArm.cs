using Cashew.Utility.Extensions;
using DigitalRuby.Tween;
using UnityEngine;

class PistonArm : MonoBehaviour
{
    const float ForceMultiplier = 100;

    Rigidbody _loadedBall;
    Vector3 _originalPosition;
    bool _launching;

    public Transform Piston;
    //public Rigidbody Piston;
    public float ArmLength;
    public float PushTime;
    public float ReturnTime;
    public float Force = 10;
    public Vector3 LocalDirection = new Vector3(-1, 0, 0);

    void Start()
    {
        _originalPosition = Piston.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<Ball>();
        if (ball == null)
            ball = other.GetComponentInParent<Ball>();
        if (ball == null)
            return;

        var rb = other.GetComponent<Rigidbody>() ;
        if (rb == null) 
            rb = other.GetComponentInParent<Rigidbody>();

        _loadedBall = rb;
    }

    public void Launch()
    {
        if (_launching)
            return;

        _originalPosition = Piston.localPosition;
        _launching = true;

        TweenFactory.Tween(
            new object(),
            Piston.localPosition,
            Piston.localPosition.WithNewX(x => x - ArmLength),
            PushTime,
            TweenScaleFunctions.Linear,
            p => Piston.localPosition = p.CurrentValue,
            p => OnComplete());

        void OnComplete()
        {
            if (_loadedBall != null)
            {
                var dir = Piston.TransformDirection(LocalDirection * Force * ForceMultiplier);
                _loadedBall.AddForce(dir);
            }
            _loadedBall = null;

            TweenFactory.Tween(
                new object(),
                Piston.transform.localPosition,
                _originalPosition,
                ReturnTime,
                TweenScaleFunctions.Linear,
                p => Piston.localPosition = p.CurrentValue,
                p => _launching = false);
        }
    }

    void Update()
    {
        if (!_launching)
            Piston.localPosition = _originalPosition;
    }
}