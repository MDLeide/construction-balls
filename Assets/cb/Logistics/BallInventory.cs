using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Sirenix.OdinInspector;
using UnityEngine;

class BallInventory : MonoBehaviour
{
    protected virtual bool HideBaseFields => false;

    [HideIf(nameof(HideBaseFields))]
    public MixedColorCanister InternalStorage;
    [HideIf(nameof(HideBaseFields))]
    public List<Canister> AttachedStorage;

    [HideIf(nameof(HideBaseFields))]
    public bool PreferInternalStorage = true;
    [HideIf(nameof(HideBaseFields))]
    public bool LockInternalStorage;

    public EventHandler<BallInventoryChangedEventArgs> BallReceived;

    public int Blue => GetQuantity(BallColor.Blue);
    public int Red => GetQuantity(BallColor.Red);
    public int Yellow => GetQuantity(BallColor.Yellow);

    public int Green => GetQuantity(BallColor.Green);
    public int Orange => GetQuantity(BallColor.Orange);
    public int Purple => GetQuantity(BallColor.Purple);


    public virtual int GetQuantity(BallColor color)
    {
        return GetInternalStorageQuantity(color) +
               GetAttachedStorageQuantity(color);
    }


    public bool CanPay(BallCost cost)
    {
        foreach (var color in Ball.AllColors)
            if (!CanPay(color, cost.GetCost(color)))
                return false;
        return true;
    }
    
    public bool CanPay(BallColor color, int amount)
    {
        // it doesn't actually matter if we are preferring internal
        // storage or otherwise, since we are simply checking against
        // total balls available, but i will leave it in anyway

        var remaining = amount;

        if (PreferInternalStorage && !LockInternalStorage)
        {
            remaining -= GetInternalStorageQuantity(color);
            if (remaining <= 0)
                return true;

            remaining -= GetAttachedStorageQuantity(color);
            return remaining <= 0;
        }
        
        remaining -= GetAttachedStorageQuantity(color);

        if (remaining <= 0)
            return true;

        if (!LockInternalStorage)
            remaining -= GetInternalStorageQuantity(color);

        return remaining <= 0;
    }

    public bool Pay(BallCost cost)
    {
        if (!CanPay(cost))
            return false;

        foreach (var color in Ball.AllColors)
            Pay(color, cost.GetCost(color));

        return true;
    }

    public bool Pay(BallColor color, int amount)
    {
        if (!CanPay(color, amount))
            return false;

        var remaining = amount;

        if (PreferInternalStorage && !LockInternalStorage)
        {
            remaining = PayInternalStorage(color, remaining);
            if (remaining <= 0)
                return true;

            remaining = PayAttachedStorage(color, remaining);
            if (remaining <= 0)
                return true;

            // we couldn't pay it all, but CanPay said we could.
            // throw an error since we mutated all the inventories
            throw new InvalidOperationException();
        }
        else
        {
            remaining = PayAttachedStorage(color, remaining);
            if (remaining <= 0)
                return true;

            if (!LockInternalStorage)
                remaining = PayInternalStorage(color, remaining);

            if (remaining > 0)
                throw new InvalidOperationException(); // CanPay should have returned false

            return true;
        }
    }

    public bool CanAdd(BallColor color)
    {
        if (PreferInternalStorage && !LockInternalStorage)
        {
            if (CanAddToInternal(color))
                return true;
            return CanAddToAttached(color);
        }
        else
        {
            if (CanAddToAttached(color))
                return true;

            if (!LockInternalStorage)
                return CanAddToInternal(color);
            return false;
        }
    }

    public bool Add(BallColor color)
    {
        if (TryAdd(color))
        {
            OnBallReceived(color);
            return true;
        }

        return false;
    }

    public bool Add(Ball ball)
    {
        return Add(ball.Color);
    }

    bool TryAdd(BallColor color)
    {
        if (!CanAdd(color))
            return false;

        if (PreferInternalStorage && !LockInternalStorage)
        {
            if (!AddToInternal(color))
                return AddToAttached(color);

            return true;
        }

        if (AddToAttached(color))
            return true;

        if (!LockInternalStorage)
            return AddToInternal(color);

        return false;
    }


    protected virtual void OnBallReceived(BallColor color)
    {
        BallReceived?.Invoke(this, new BallInventoryChangedEventArgs(color, 1));
    }

    public virtual int GetInternalStorageQuantity(BallColor color)
    {
        return InternalStorage.GetQuantity(color);
    }

    public virtual int GetAttachedStorageQuantity(BallColor color)
    {
        return AttachedStorage.Where(p => p.Color == color).Sum(p => p.Quantity);
    }

    public virtual int PayInternalStorage(BallColor color, int quantity)
    {
        return InternalStorage.Pay(color, quantity);
    }

    public virtual int PayAttachedStorage(BallColor color, int quantity)
    {
        foreach (var attached in AttachedStorage.Where(p => p.Color == color))
        {
            quantity = attached.Subtract(quantity);
            if (quantity <= 0)
                return 0;
        }

        return quantity;
    }

    public virtual bool CanAddToAttached(BallColor color)
    {
        foreach (var attached in AttachedStorage.Where(p => p.Color == color))
            if (attached.CanAdd())
                return true;
        return false;
    }

    public virtual bool CanAddToInternal(BallColor color)
    {
        return InternalStorage.CanAdd(color);
    }

    public virtual bool AddToInternal(BallColor color)
    {
        return InternalStorage.Add(color);
    }

    public virtual bool AddToAttached(BallColor color)
    {
        foreach (var ballStorage in AttachedStorage.Where(p => p.Color == color))
            if (ballStorage.Add())
                return true;

        return false;
    }


    void Reset()
    {
        InternalStorage = gameObject.GetComponent<MixedColorCanister>();
        if (InternalStorage == null && !typeof(InventoryProxy).IsAssignableFrom(GetType()))
            InternalStorage = gameObject.AddComponent<MixedColorCanister>();
    }
}