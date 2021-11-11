using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using UnityEngine;


class BlockNavigator
{
    public static bool AreConnected(Block a, Block b)
    {
        return AreConnected(a, b, new List<Block> {a});
    }

    static bool AreConnected(Block a, Block b, List<Block> checkedBlocks)
    {
        if (a == b)
            return true;

        foreach (var neighbor in a.Neighbors)
        {
            if (neighbor.Value == b)
                return true;

            if (!checkedBlocks.Contains(neighbor.Value))
            {
                checkedBlocks.Add(neighbor.Value);
                if (AreConnected(neighbor.Value, b, checkedBlocks))
                    return true;
            }
        }

        return false;
    }

    public static Block GetTopBlock(Block block)
    {
        var ray = new Ray(block.MidPoint, Vector3.up);
        if (Physics.Raycast(ray, out var hit, Game.UnitDistance))
        {
            var b = hit.transform.GetComponentAnywhere<Block>();
            if (b == block)
                return block;

            if (b != null)
                return GetTopBlock(b);
        }

        return block;
    }

    public static Block GetBaseBlock(Block block)
    {
        var ray = new Ray(block.MidPoint, Vector3.down);
        if (Physics.Raycast(ray, out var hit, Game.UnitDistance))
        {
            var b = hit.transform.GetComponentAnywhere<Block>();
            if (b == block)
                return block;

            if (b != null)
                return GetBaseBlock(b);
        }

        return block;
    }
}