using System;
using System.Collections.Generic;
using DigitalRuby.Tween;
using Sirenix.OdinInspector;
using UnityEngine;

class PickUp : MonoBehaviour
{
    int _cancelAnimations;

    // how fast the pickup moves when being placed/picked up
    const float PickUpSpeed = 10f;

    readonly object _placePositionKey = new object();
    readonly object _placeRotationKey = new object();

    public Rigidbody RB;
    public Collider Collider;

    [Space]
    public bool IsPlacing;
    public bool IsPlaced;
    public bool IsOnPallet;

    public bool IsLocked;

    public event EventHandler<PickUpPlacingEventArgs> BeingPlaced;
    public event EventHandler PickedUp;
    public event EventHandler Released;
    public event EventHandler Placed;

    public PickUpType Type = PickUpType.Default;

    void Start()
    {
        if (IsPlaced && IsLocked && !RB.isKinematic)
            Debug.LogWarning($"pickup {name} is placed and locked but the rigidbody is not kinematic.");


        if (Type == PickUpType.Default)
            Debug.LogWarning($"PickUp {gameObject.name} has a default pick up type. Assign a pick up type.");
    }

    public void CancelAnimations()
    {
        // todo: there are probably several bugs here
        // for instance, BlockProxy listens for a being placed event
        // and relies on a subsequent placed event to do some things
        // if the animations are canceled and placed is not called
        // we will get bad behavior
        // that doesn't happen anywhere, yet, but should be considered
        // if this gets called somewhere else in the future

        TweenFactory.Tween(
            _placeRotationKey,
            0,
            0,
            .01f,
            TweenScaleFunctions.Linear,
            p => { });

        TweenFactory.Tween(
            _placePositionKey,
            0,
            0,
            .01f,
            TweenScaleFunctions.Linear,
            p => { });
    }

    public void Grab(Transform newParent)
    {
        transform.parent = newParent;

        var targetPosition = Vector3.zero;
        // raise balls up a bit so they sit in a better spot
        if (GetComponent<Ball>() != null)
            targetPosition = new Vector3(0, .3f, 0);

        TweenFactory.Tween(
            _placePositionKey,
            transform.localPosition,
            targetPosition,
            .25f,
            TweenScaleFunctions.CubicEaseInOut,
            p => transform.localPosition = p.CurrentValue);

        transform.localRotation = Quaternion.identity;

        RB.isKinematic = true;
        if (Collider != null)
            Collider.enabled = false;

        IsPlaced = false;
        IsOnPallet = false;

        PickedUp?.Invoke(this, new EventArgs());
    }

    public void Release()
    {
        CancelAnimations();

        transform.parent = null;

        RB.isKinematic = false;
        if (Collider != null)
        {
            Collider.enabled = true;
            var meshCollider = Collider.GetComponent<MeshCollider>();
            if (meshCollider != null)
                meshCollider.convex = true;
        }

        IsPlaced = false;
        IsOnPallet = false;

        Released?.Invoke(this, new EventArgs());
    }

    public void Place(Transform targetTransform, bool useLocalPosition = false)
    {
        Place(targetTransform.position, targetTransform.rotation, useLocalPosition);
    }

    public void Place(Vector3 position, Quaternion? rotation = null, bool useLocalPosition = false)
    {
        IsPlacing = true;
        BeingPlaced?.Invoke(this, new PickUpPlacingEventArgs(position));

        var distance = (position - transform.position).magnitude;

        if (useLocalPosition)
            TweenFactory.Tween(
                _placePositionKey,
                transform.localPosition,
                position,
                distance / PickUpSpeed, // duration
                TweenScaleFunctions.CubicEaseInOut,
                p => transform.localPosition = p.CurrentValue,
                p =>
                {
                    IsPlacing = false;
                    IsPlaced = true;
                    RB.isKinematic = true;
                    if (Collider != null)
                        Collider.enabled = true;
                    Placed?.Invoke(this, new EventArgs());
                });
        else
            TweenFactory.Tween(
                _placePositionKey,
                transform.position,
                position,
                distance / PickUpSpeed, // duration
                TweenScaleFunctions.CubicEaseInOut,
                p => transform.position = p.CurrentValue,
                p =>
                {
                    IsPlacing = false;
                    IsPlaced = true;
                    RB.isKinematic = true;
                    if (Collider != null)
                        Collider.enabled = true;
                    Placed?.Invoke(this, new EventArgs());
                });

        if (rotation.HasValue)
        {
            if (useLocalPosition)
                TweenFactory.Tween(
                    _placeRotationKey,
                    transform.localRotation,
                    rotation.Value,
                    distance / PickUpSpeed,
                    TweenScaleFunctions.CubicEaseInOut,
                    p => transform.localRotation = p.CurrentValue);
            else
                TweenFactory.Tween(
                    _placeRotationKey,
                    transform.rotation,
                    rotation.Value,
                    distance / PickUpSpeed,
                    TweenScaleFunctions.CubicEaseInOut,
                    p => transform.rotation = p.CurrentValue);
        }
    }

    void Reset()
    {
        RB = GetComponent<Rigidbody>();
        if (RB == null)
            RB = gameObject.AddComponent<Rigidbody>();

        Collider = GetComponent<Collider>();
        if (Collider == null)
            Collider = GetComponentInChildren<Collider>();
    }
}