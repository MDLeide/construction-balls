using System;
using System.Linq;
using UnityEngine;

class NetworkBlock : MonoBehaviour
{
    public bool NetworkOwner;
    public Block Block;
    public BlockNetwork BlockNetwork;

    public event EventHandler NetworkUpdated; 

    void Start()
    {
     //   Block.PickUp.Placed += BlockOnPlaced;
        Block.NeighborsUpdated += BlockOnPlaced;
        Block.StabilityLost += BlockOnStabilityLost;
        Block.PickUp.PickedUp += PickUpOnPickedUp;
    }

    void PickUpOnPickedUp(object sender, EventArgs e)
    {
        BlockMoved();
    }

    void BlockOnStabilityLost(object sender, EventArgs e)
    {
        BlockMoved();
    }

    void BlockMoved()
    {
        if (BlockNetwork == null)
            return;

        BlockNetwork.Remove(this);
    }

    static int count = 0;

    void BlockOnPlaced(object sender, EventArgs e)
    {
        if (Block.PickUp.IsOnPallet)
            return;

        count++;
        if (count == 2)
        {
            count = 2;
        }

        var networkBlocks =
            Block.Neighbors.Values
                .Select(p => p.gameObject.GetComponent<NetworkBlock>())
                .Where(p => p != null)
                .ToArray();

        var networks = networkBlocks.Select(p => p.BlockNetwork).Distinct().ToArray();

        if (networks.Length == 0)
        {
            InitializeBlockNetwork();
        }
        else if (networks.Length == 1)
        {
            BlockNetwork = networks.First();
            if (BlockNetwork == null)
                InitializeBlockNetwork();
            else
                BlockNetwork.Add(this);
        }
        else
        {
            BlockNetwork = BlockNetwork.Merge(networks);
            BlockNetwork.Add(this);
        }

        NetworkUpdated?.Invoke(this, new EventArgs());
    }

    public void InitializeBlockNetwork()
    {
        var obj = new GameObject($"block network ?");
        obj.transform.parent = transform;
        BlockNetwork = obj.AddComponent<BlockNetwork>();
        BlockNetwork.Initialize(this);

        NetworkOwner = true;

        var debug = obj.AddComponent<BlockNetworkDebug>();
        debug.BlockNetwork = BlockNetwork;
    }
}