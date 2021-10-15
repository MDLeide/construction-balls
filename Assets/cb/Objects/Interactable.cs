using System;
using DigitalRuby.Tween;
using Sirenix.OdinInspector;
using TreeEditor;
using UnityEngine;

class Interactable : MonoBehaviour
{
    bool _buffered;
    RaycastHit? _bufferedHit;

    Vector3 _original;
    Vector3 _pushed;
    Vector3 _failed;

    float _nextInteraction;

    public string Message;
    public float InteractionCooldown = .65f;

    [Header("Input Buffer")]
    public bool BufferInput = true;
    public float BufferLength = .1f;
    [Header("Animation")]
    public bool PlayAnimation = false;

    [FoldoutGroup("Time", VisibleIf = "PlayAnimation")]
    public float SuccessPushTime = .15f;
    [FoldoutGroup("Time")]
    public float SuccessReturnTime = .5f;
    [FoldoutGroup("Time")]
    public float FailurePushTime = .1f;
    [FoldoutGroup("Time")]
    public float FailureReturnTime = .1f;

    [Space]
    [FoldoutGroup("Distance", VisibleIf = "PlayAnimation")]
    public float SuccessPushDistance = .04f;
    [FoldoutGroup("Distance")]
    public float FailurePushDistance = .01f;
    
    [Header("Availability")]
    [ReadOnly]
    public bool IsCoolingDown;
    // set by users to indicate the button is turned off
    public bool IsAvailable = true;

    public event EventHandler<InteractablePushedEventArgs> InteractedWith;
    public event EventHandler<InteractablePushedEventArgs> Pushed;
    
    void Start()
    {
        if (PlayAnimation)
        {
            _original = transform.position;

            if (transform.parent != null)
            {
                _pushed = transform.position - transform.parent.forward * SuccessPushDistance;
                _failed = transform.position - transform.parent.forward * FailurePushDistance;
            }
            else
            {
                _pushed = transform.position - transform.forward * SuccessPushDistance;
                _failed = transform.position - transform.forward * FailurePushDistance;
            }
        }
    }

    void Update()
    {
        if (IsCoolingDown && _nextInteraction <= Time.time)
        {
            IsCoolingDown = false;

            if (_buffered)
                Interact(_bufferedHit);
            _buffered = false;
        }
    }

    [Button]
    public void Interact()
    {
        Interact(null);
    }

    public virtual void Interact(RaycastHit? hit)
    {
        if (IsCoolingDown)
        {
            if (_nextInteraction - Time.time <= BufferLength)
            {
                _buffered = true;
                _bufferedHit = hit;
            }
            return;
        }

        if (!IsAvailable)
        {
            if (PlayAnimation)
                PushFail();
            return;
        }

        if (PlayAnimation)
            PushSuccess();

        InteractedWith?.Invoke(this, new InteractablePushedEventArgs(hit));
        Pushed?.Invoke(this, new InteractablePushedEventArgs(hit));

        if (InteractionCooldown > 0)
        {
            _nextInteraction = Time.time + InteractionCooldown;
            IsCoolingDown = true;
        }
    }

    void PushFail()
    {
        TweenFactory.Tween(
            this,
            transform.position,
            _failed,
            .1f,
            TweenScaleFunctions.CubicEaseOut,
            p => transform.position = p.CurrentValue,
            p =>
            {
                TweenFactory.Tween(
                    this,
                    transform.position,
                    _original,
                    FailureReturnTime,
                    TweenScaleFunctions.Linear,
                    ip => transform.position = ip.CurrentValue);
            });
    }

    void PushSuccess()
    {
        TweenFactory.Tween(
            this,
            transform.position,
            _pushed,
            .15f,
            TweenScaleFunctions.CubicEaseOut,
            p => transform.position = p.CurrentValue,
            p =>
            {
                TweenFactory.Tween(
                    this,
                    transform.position,
                    _original,
                    SuccessReturnTime,
                    TweenScaleFunctions.Linear,
                    ip => transform.position = ip.CurrentValue);
            });
    }
}