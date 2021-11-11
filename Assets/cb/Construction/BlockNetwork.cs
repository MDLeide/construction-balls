using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew;
using UnityEngine;
using UnityEngine.Networking.Types;
using Random = UnityEngine.Random;

class BlockNetwork : MonoBehaviour
{
    public static int NetworkCount = 0;
    // must be even!!!
    const int GrowBy = 2;

    int _count;
    Vector3Int _offset;

    public CashewGrid Grid;
    public NetworkBlock[,,] Network;
    public NetworkBlock OriginalBlock;
    public List<NetworkBlock> Blocks = new List<NetworkBlock>();
    public int NetworkId;

    public bool IsEmpty => _count <= 0;

    void Start()
    {
        NetworkCount++;
        NetworkId = NetworkCount;
        name = $"block network {NetworkId}";
    }

    public void Initialize(NetworkBlock block)
    {
        Grid = new CashewGrid();
        
        Grid.Center = new Vector3(
            block.Block.TargetPosition.x - Grid.HalfUnitDistance,
            block.Block.TargetPosition.y - Grid.HalfUnitDistance,
            block.Block.TargetPosition.z - Grid.HalfUnitDistance);

        Network = new NetworkBlock[1,1,1];
        Network[0, 0, 0] = block;

        OriginalBlock = block;
        Blocks.Add(block);

        _count++;
    }

    public NetworkBlock GetReplacementOwner(NetworkBlock currentOwner)
    {
        for (int x = 0; x < Network.GetLength(0); x++)
        for (int y = 0; y < Network.GetLength(1); y++)
        for (int z = 0; z < Network.GetLength(2); z++)
        {
            if (Network[x, y, z] != null && Network[x, y, z] != currentOwner)
                return Network[x, y, z];
        }

        return null;
    }

    // removes a block from the network
    // if that results in the network being empty
    // destroys itself and returns true
    public void Remove(NetworkBlock block)
    {
        _count--;
        Blocks.Remove(block);
        if (!Blocks.Any())
        {
            Destroy(gameObject);
            return;
        }

        if (block == OriginalBlock)
        {
            block.NetworkOwner = false;
            OriginalBlock = Blocks.First();
            OriginalBlock.NetworkOwner = true;
            transform.parent = OriginalBlock.transform;
        }

        var cell = Grid.GetCell(block.transform.position);
        if (cell.x > 0)
            cell.x--;
        if (cell.y > 0)
            cell.y--;
        if (cell.z > 0)
            cell.z--;

        var address = cell + _offset;
        Network[address.x, address.y, address.z] = null;
        TrimNetwork();

        CheckForSplitNetwork();
    }

    // checks to see if the network has been split somewhere, and if it has
    // creates new networks, destroying this original one
    // definitely a better algorithm for this, but this was very easy to implement
    // returns true if the network was split
    public void CheckForSplitNetwork()
    {
        var networks = new Dictionary<NetworkBlock, List<NetworkBlock>>();

        var queue = new Queue<NetworkBlock>(Blocks);
        networks.Add(queue.Dequeue(), new List<NetworkBlock>());

        // check each block for attached blocks to determine sub-networks
        while (queue.Any())
        {
            var current = queue.Dequeue();
            var found = false;

            foreach (var net in networks)
                if (net.Key.Block.IsConnectedTo(current.Block))
                {
                    net.Value.Add(current);
                    found = true;
                    break;
                }

            if (!found)
                networks.Add(current, new List<NetworkBlock>());
        }

        if (networks.Count == 1)
            return;

        // destroy this network and create new ones for each sub-network
        Destroy(gameObject);

        foreach (var network in networks)
        {
            network.Key.InitializeBlockNetwork();
            foreach (var block in network.Value)
                network.Key.BlockNetwork.Add(block);
        }
    }

    public bool DestroyIfEmpty()
    {
        if (IsEmpty)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    void TrimNetwork()
    {
        GetMinMax(out var min, out var max);

        _offset -= min;

        var network = new NetworkBlock[max.x - min.x + 1, max.y - min.y + 1, max.z - min.z + 1];

        for (int x = 0; x < network.GetLength(0); x++)
        for (int y = 0; y < network.GetLength(1); y++)
        for (int z = 0; z < network.GetLength(2); z++)
            network[x, y, z] = Network[x + min.x, y + min.y, z + min.z];

        Network = network;
    }

    void GetMinMax(out Vector3Int min, out Vector3Int max)
    {
        min = new Vector3Int(
            int.MaxValue,
            int.MaxValue,
            int.MaxValue);

        max = new Vector3Int(
            -int.MaxValue,
            -int.MaxValue,
            -int.MaxValue);
        
        for (int x = 0; x < Network.GetLength(0); x++)
        {
            for (int y = 0; y < Network.GetLength(1); y++)
            {
                for (int z = 0; z < Network.GetLength(2); z++)
                {
                    if (Network[x, y, z] != null)
                    {
                        if (x < min.x)
                            min.x = x;
                        if (y < min.y)
                            min.y = y;
                        if (z < min.z)
                            min.z = z;

                        if (x > max.x)
                            max.x = x;
                        if (y > max.y)
                            max.y = y;
                        if (z > max.z)
                            max.z = z;
                    }
                }
            }
        }
    }

    public void Add(NetworkBlock block)
    {
        _count++;

        Blocks.Add(block);
        block.BlockNetwork = this;

        var cell = Grid.GetCell(block.Block.TargetPosition);
        if (cell.x > 0)
            cell.x--;
        if (cell.y > 0)
            cell.y--;
        if (cell.z > 0)
            cell.z--;

        var address = cell + _offset;

        while (address.x < 0)
        {
            PadFront(new Vector3Int(1, 0, 0));
            address.x++;
            _offset.x++;
        }

        while (address.y < 0)
        {
            PadFront(new Vector3Int(0, 1, 0));
            address.y++;
            _offset.y++;
        }

        while (address.z < 0)
        {
            PadFront(new Vector3Int(0, 0, 1));
            address.z++;
            _offset.z++;
        }

        while (address.x >= Network.GetLength(0))
        {
            PadBack(new Vector3Int(1,0,0));
        }

        while (address.y >= Network.GetLength(1))
        {
            PadBack(new Vector3Int(0, 1, 0));
        }

        while (address.z >= Network.GetLength(2))
        {
            PadBack(new Vector3Int(0, 0, 1));
        }

        Network[address.x, address.y, address.z] = block;
    }

    void PadFront(Vector3Int increase)
    {
        var newArray = new NetworkBlock[
            Network.GetLength(0) + increase.x,
            Network.GetLength(1) + increase.y,
            Network.GetLength(2) + increase.z];

        for (int x = 0; x < Network.GetLength(0); x++)
        for (int y = 0; y < Network.GetLength(1); y++)
        for (int z = 0; z < Network.GetLength(2); z++)
            newArray[x + increase.x, y + increase.y, z + increase.z] =
                Network[x, y, z];

        Network = newArray;
    }

    void PadBack(Vector3Int increase)
    {
        var newArray = new NetworkBlock[
            Network.GetLength(0) + increase.x,
            Network.GetLength(1) + increase.y,
            Network.GetLength(2) + increase.z];

        for (int x = 0; x < Network.GetLength(0); x++)
        for (int y = 0; y < Network.GetLength(1); y++)
        for (int z = 0; z < Network.GetLength(2); z++)
            newArray[x, y, z] = Network[x, y, z];

        Network = newArray;
    }

    void Resize()
    {
        var newArray = new NetworkBlock[
            Network.GetLength(0) + GrowBy,
            Network.GetLength(1) + GrowBy,
            Network.GetLength(2) + GrowBy];

        for (int x = 0; x < Network.GetLength(0); x++)
        for (int y = 0; y < Network.GetLength(1); y++)
        for (int z = 0; z < Network.GetLength(2); z++)
            newArray[x + GrowBy / 2, y + GrowBy / 2, z + GrowBy / 2] =
                Network[x, y, z];

        Network = newArray;
    }

    public static BlockNetwork Merge(BlockNetwork[] networks)
    {
        networks[0].OriginalBlock.InitializeBlockNetwork();
        var net = networks[0].OriginalBlock.BlockNetwork;

        for (int i = 0; i < networks.Length; i++)
        {
            for (int x = 0; x < networks[i].Network.GetLength(0); x++)
            for (int y = 0; y < networks[i].Network.GetLength(1); y++)
            for (int z = 0; z < networks[i].Network.GetLength(2); z++)
                if (networks[i].Network[x, y, z] != null && networks[i].Network[x,y,z] != networks[0].OriginalBlock)
                    net.Add(networks[i].Network[x, y, z]);
        }

        for (int i = 0; i < networks.Length; i++)
            Destroy(networks[i].gameObject);

        return net;
    }
}

/*
 * 
    void Update()
    {
        for (int x = 1; x <= DrawDistance; x++)
        for (int y = 1; y <= DrawDistance; y++)
        for (int z = 1; z <= DrawDistance; z++)
        {
            DrawCell(Grid.GetCenterOfCell(x, y, z), Grid.HalfUnitDistance, Color.blue);
            DrawCell(Grid.GetCenterOfCell(-x, y, z), Grid.HalfUnitDistance, Color.red);
            DrawCell(Grid.GetCenterOfCell(x, y, -z), Grid.HalfUnitDistance, Color.yellow);
            DrawCell(Grid.GetCenterOfCell(-x, y, -z), Grid.HalfUnitDistance, Color.green);

            DrawCell(Grid.GetCenterOfCell(x, y, z), Grid.HalfUnitDistance * .05f, Color.black);
            DrawCell(Grid.GetCenterOfCell(-x, y, z), Grid.HalfUnitDistance * .15f, Color.white);
            DrawCell(Grid.GetCenterOfCell(x, y, -z), Grid.HalfUnitDistance * .25f, Color.black);
            DrawCell(Grid.GetCenterOfCell(-x, y, -z), Grid.HalfUnitDistance * .35f, Color.white);
        }

        var aCell = Grid.GetCell(Pointer.transform.position);
        var aCellCenter = Grid.GetCenterOfCell(aCell);

        DrawHelper.DrawCircle(aCellCenter, .35f, CircleSegments, Color.yellow);
        DrawHelper.DrawCircle(aCellCenter, .35f, CircleSegments, Color.yellow, 1);
        DrawHelper.DrawCircle(aCellCenter, .35f, CircleSegments, Color.yellow, 2);

        var activeCell = Grid.GetCenterOfCell(Pointer.transform.position);
        var activeCellPosition = Grid.GetBottomOfCell(activeCell);
        var activeCellCenter = Grid.GetCenterOfCell(activeCell);

        DrawHelper.DrawCircle(activeCellCenter, CircleRadius, CircleSegments, Color.white);
        DrawHelper.DrawCircle(activeCellCenter, CircleRadius, CircleSegments, Color.white, 1);
        DrawHelper.DrawCircle(activeCellCenter, CircleRadius, CircleSegments, Color.white, 2);
        DrawHelper.DrawBox(activeCellPosition, Vector3.one * .25f, Color.magenta);
    }

*/