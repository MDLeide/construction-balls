using System;
using Cashew.Utility.Extensions;

class ConnectionInventoryProxy : InventoryProxy
{
    public override BallInventory Inventory
    {
        get
        {
            if (InventoryProvider == null)
                return null;
            return InventoryProvider.Inventory;
        }
    }

    public InventoryProvider InventoryProvider;
    public ConnectionBlock ConnectionBlock;

    protected override void Start()
    {
        ConnectionBlock.ChainUpdated += ChainUpdated;
        base.Start();
    }

    void ChainUpdated(object sender, EventArgs e)
    {
        var otherSide = ConnectionBlock.GetOtherSide();

        InventoryProvider = otherSide.GetComponentAnywhere<InventoryProvider>();
    }
}