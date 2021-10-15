using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PickUp))]
class Ball : MonoBehaviour
{
    float _destroy;
    bool _lifetimeActive = true;

    public BallColor Color;
    public Collider Collider;
    public Rigidbody RB;
    public PickUp PickUp;
    public float Lifetime = 300;
    public MeshRenderer MeshRenderer;

    void Start()
    {
        _lifetimeActive = true;
        _destroy = Time.time + Lifetime;

        PickUp = GetComponent<PickUp>();
        RB = GetComponent<Rigidbody>();

        transform.parent = Game.Instance.BallContainer;
        PickUp.PickedUp += (s, e) => PauseLifetime();
        PickUp.Released += (s, e) => ResumeLifetime();
        PickUp.Placed += (s, e) => PauseLifetime();

        MeshRenderer = GetComponent<MeshRenderer>();
        if (MeshRenderer == null)
            MeshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        if (_lifetimeActive && _destroy <= Time.time)
            Destroy(gameObject);
    }

    public void PauseLifetime()
    {
        _lifetimeActive = false;
    }

    public void ResetLifetime()
    {
        _destroy = Time.time + Lifetime;
    }

    public void ResumeLifetime()
    {
        ResetLifetime();
        _lifetimeActive = true;
    }
    
    public void GhostMode()
    {
        RB.isKinematic = true;
        Collider.enabled = false;
        RB.velocity = Vector3.zero;
        MeshRenderer.enabled = false;
    }

    public void TurnOffGhostMode()
    {
        RB.isKinematic = false;
        Collider.enabled = true;
        MeshRenderer.enabled = true;
    }
}