using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;


class PlayerHands : MonoBehaviour
{
    const int ActionCooldownFrames = 10;
    int _nextFrame;
    bool _sameFrame;

    public PlayerEyes Eyes;
    public PlayerController Controller;
    
    [Header("Grab")]
    public Transform HoldPosition;
    public Transform HeldItemRotation;

    [Space]
    public float ThrowPowerMin;

    public float ThrowPowerMax;
    [Space]
    [ReadOnly]
    public PickUp HeldObject;
    [ShowInInspector]
    public Block HeldBlock
    {
        get
        {
            if (HeldObject == null)
                return null;
            return HeldObject.GetComponent<Block>();
        }
    }
    
    public event EventHandler<PickUpMovedEventArgs> Grabbed;
    public event EventHandler<PickUpMovedEventArgs> Released;
    public event EventHandler<PickUpMovedEventArgs> Placed;
    public event EventHandler<PickUpMovedEventArgs> PlacedOnPallet;

    void Update()
    {
        _sameFrame = false;
        var rot = new Vector3(
            Controller.Look.localEulerAngles.x,
            0,
            0);

        if (rot.x > 180)
            rot.x -= 360;

        rot.x /= 1.5f;

        HeldItemRotation.localRotation = Quaternion.Euler(rot);
    }

    public void Interact()
    {
        if (CanInteract())
            Eyes.InteractableInSights.Interact(Eyes.Hit);
    }

    public void Grab()
    {
        if (!CanGrab())
            return;

        Grab(Eyes.PickupInSights);
    }

    public void GrabFromTop()
    {
        if (!CanGrab())
            return;

        if (Eyes.BlockInSights == null)
            return;

        var top = Eyes.BlockInSights.GetTopBlock();
        Grab(top.PickUp);
    }

    // forces a grab, does not validate input
    // called after validation in other methods,
    // also used for reloading
    public void Grab(PickUp pickup)
    {
        if (HeldObject != null)
        {
            HeldObject.CancelAnimations();
            Release();
        }

        HeldObject = pickup;
        HeldObject.Grab(HoldPosition);
        _sameFrame = true;

        Grabbed?.Invoke(this, new PickUpMovedEventArgs(HeldObject));
    }

    public void Take()
    {
        if (!CanTake())
            return;
        
        HeldObject = Eyes.PalletInSights.Take();
        if (HeldObject == null)
            return;

        HeldObject.Grab(HoldPosition);
        _sameFrame = true;

        Grabbed?.Invoke(this, new PickUpMovedEventArgs(HeldObject));
    }

    public void Throw(float throwPower)
    {
        if (!CanThrow())
            return;

        var obj = HeldObject;
        Release();
        obj.RB.AddForce(Eyes.LookTransform.forward * throwPower);
    }

    public void Release()
    {
        if (!CanRelease())
            return;

        var args = new PickUpMovedEventArgs(HeldObject);

        HeldObject.Release();
        HeldObject = null;
        _sameFrame = true;

        Released?.Invoke(this, args);
    }

    public void PlaceOnBlock()
    {
        if (!CanPlaceOnBlock() || _nextFrame > Time.frameCount)
            return;

        _nextFrame = Time.frameCount + ActionCooldownFrames;

        var args = new PickUpMovedEventArgs(HeldObject);

        Eyes.BlockInSights.PlaceOn(HeldBlock, Eyes.Hit);
        HeldObject = null;
        _sameFrame = true;

        Placed?.Invoke(this, args);
    }

    public void PlaceOnPallet()
    {
        if (!CanPlaceOnPallet())
            return;

        var args = new PickUpMovedEventArgs(HeldObject);

        Eyes.PalletInSights.Place(HeldObject);
        HeldObject = null;
        _sameFrame = true;

        PlacedOnPallet?.Invoke(this, args);
    }

    public void PlaceOnGround()
    {
        if (!CanPlaceOnGround())
            return;

        var args = new PickUpMovedEventArgs(HeldObject);

        Eyes.PanelInSights.Place(HeldBlock, Eyes.Hit);
        HeldObject = null;
        _sameFrame = true;

        Placed?.Invoke(this, args);
    }

    public bool CanThrow()
    {
        return enabled && 
               HeldObject != null;
    }
    
    public bool CanPlaceOnBlock()
    {
        return enabled &&
               HeldBlock != null &&
               Eyes.BlockInSights != null &&
               Eyes.BlockInSights.CanPlace(Eyes.Hit) && 
                !_sameFrame;
    }

    public bool CanPlaceOnPallet()
    {
        return enabled && 
               HeldObject != null &&
               Eyes.PalletInSights != null &&
               Eyes.PalletInSights.CanPlace(HeldObject) &&
               Eyes.InteractableInSights == null && // prefer to interact
               !_sameFrame;
    }

    public bool CanPlaceOnGround()
    {
        return enabled && 
               HeldBlock != null && 
               Eyes.PanelInSights != null && 
               Eyes.PanelInSights.CanPlace(Eyes.Hit) &&
               !_sameFrame;
    }

    public bool CanTake()
    {
        return enabled && 
               HeldObject == null && 
               Eyes.PalletInSights != null &&
               Eyes.PalletInSights.CanTake() && 
               !_sameFrame;
    }

    public bool CanRelease()
    {
        return enabled && 
               HeldObject != null &&
               !_sameFrame;
    }

    public bool CanGrab()
    {
        return enabled && 
               Eyes.PalletInSights == null && 
               Eyes.PickupInSights != null && 
               !Eyes.PickupInSights.IsLocked &&
               !_sameFrame;
    }

    public bool CanInteract()
    {
        return enabled && 
               Eyes.InteractableInSights != null && 
               !_sameFrame;
    }
}