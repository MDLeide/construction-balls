using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class InventoryProxy : BallInventory
{
    protected override bool HideBaseFields => true;

    public virtual BallInventory Inventory { get; private set; }

    protected virtual void Start()
    {
        if (Inventory != null)
            Inventory.BallReceived += InventoryOnBallReceived;
    }

    public void RegisterInventory(BallInventory inventory)
    {
        if (Inventory != null)
            Inventory.BallReceived -= InventoryOnBallReceived;

        Inventory = inventory;

        if (Inventory != null)
            Inventory.BallReceived += InventoryOnBallReceived;
    }

    void InventoryOnBallReceived(object sender, BallInventoryChangedEventArgs e)
    {
        OnBallReceived(e.Color);
    }

    public override int GetQuantity(BallColor color)
    {
        if (Inventory == null)
            return 0;

        return Inventory.GetQuantity(color);
    }

    public override int GetInternalStorageQuantity(BallColor color)
    {
        if (Inventory == null)
            return 0;
        return Inventory.GetInternalStorageQuantity(color);
    }

    public override int GetAttachedStorageQuantity(BallColor color)
    {
        if (Inventory == null)
            return 0;
        return Inventory.GetAttachedStorageQuantity(color);
    }

    public override int PayInternalStorage(BallColor color, int quantity)
    {
        if (Inventory == null)
            return quantity;
        return Inventory.PayInternalStorage(color, quantity);
    }

    public override int PayAttachedStorage(BallColor color, int quantity)
    {
        if (Inventory == null)
            return quantity;
        return Inventory.PayAttachedStorage(color, quantity);
    }

    public override bool CanAddToInternal(BallColor color)
    {
        if (Inventory == null)
            return false;
        return Inventory.CanAddToInternal(color);
    }

    public override bool CanAddToAttached(BallColor color)
    {
        if (Inventory == null)
            return false;
        return Inventory.CanAddToAttached(color);
    }

    public override bool AddToAttached(BallColor color)
    {
        if (Inventory == null)
            return false;
        return Inventory.AddToAttached(color);
    }

    public override bool AddToInternal(BallColor color)
    {
        if (Inventory == null)
            return false;
        return Inventory.AddToInternal(color);
    }
}
/*
class InvProxy : MonoBehaviour
{
    public virtual BallInventory Inventory { get; set; }

    public bool HasInventory => Inventory != null;

    public int Blue => GetColorTotal(BallColor.Blue);
    public int Red => GetColorTotal(BallColor.Red);
    public int Yellow => GetColorTotal(BallColor.Yellow);

    public int Green => GetColorTotal(BallColor.Green);
    public int Orange => GetColorTotal(BallColor.Orange);
    public int Purple => GetColorTotal(BallColor.Purple);

    int GetColorTotal(BallColor color)
    {
        if (!HasInventory)
            return 0;
        return Inventory.GetQuantity(color);
    }

    public bool CanPay(BallCost cost)
    {
        if (!HasInventory)
            return false;
        return Inventory.CanPay(cost);
    }

    public bool CanPay(BallColor color, int amount)
    {
        if (!HasInventory)
            return false;
        return Inventory.CanPay(color, amount);
    }

    public bool Pay(BallCost cost)
    {
        if (!HasInventory)
            return false;
        return Inventory.Pay(cost);
    }

    public bool Pay(BallColor color, int amount)
    {
        if (!HasInventory)
            return false;
        return Inventory.Pay(color, amount);
    }

    public bool CanAdd(BallColor color)
    {
        if (!HasInventory)
            return false;
        return Inventory.CanAdd(color);
    }

    public bool Add(BallColor color)
    {
        if (!HasInventory)
            return false;
        return Inventory.Add(color);
    }

    public bool Add(Ball ball)
    {
        if (!HasInventory)
            return false;
        return Inventory.Add(ball);
    }
}*/