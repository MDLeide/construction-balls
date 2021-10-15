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

    public void Place(Transform targetTransform)
    {
        Place(targetTransform.position, targetTransform.rotation);
    }

    public void Place(Vector3 localPosition, Quaternion? rotation = null)
    {
        // an object is always placed from the hands, so the rigidbody is already kinematic
        // we will switch convex off for mesh colliders now so they can interact with balls properly
        //if (Collider != null)
        //{
        //    var meshCollider = Collider.GetComponent<MeshCollider>();

        //    if (meshCollider != null)
        //        meshCollider.convex = false;
        //}

        IsPlacing = true;
        BeingPlaced?.Invoke(this, new PickUpPlacingEventArgs(localPosition));

        var distance = (localPosition - transform.localPosition).magnitude;
        TweenFactory.Tween(
            _placePositionKey,
            transform.localPosition,
            localPosition,
            distance / PickUpSpeed,
//            .25f,
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

        if (rotation.HasValue)
            TweenFactory.Tween(
                _placeRotationKey,
                transform.rotation,
                rotation.Value,
                distance / PickUpSpeed,
                //.25f,
                TweenScaleFunctions.CubicEaseInOut,
                p => transform.rotation = p.CurrentValue);
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