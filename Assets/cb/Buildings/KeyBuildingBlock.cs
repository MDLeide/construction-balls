using System;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

class KeyBuildingBlock : MonoBehaviour
{
    public int ScanRange = 10;
    public int ScanHeight = 10;

    [Button]
    public Blueprint MakeBlueprint()
    {
        var blockArray = BuildBlockArray();
        var trimmedArray = TrimBlockArray(blockArray, out var min);
        return Blueprint.FromBlockArray(trimmedArray, new Vector3Int(ScanRange - min.x, 0, ScanRange - min.z));
    }

    BuildingBlock[,,] BuildBlockArray()
    {
        var array = new BuildingBlock[ScanRange * 2 + 1, ScanHeight, ScanRange * 2 + 1];

        var objects = Physics.OverlapBox(
            transform.position,
            new Vector3(
                ScanRange * Game.UnitDistance,
                (ScanHeight * Game.UnitDistance) / 2,
                ScanRange * Game.UnitDistance));

        foreach (var obj in objects)
        {
            var buildingBlock = obj.transform.GetComponentAnywhere<BuildingBlock>();
            if (buildingBlock == null)
                continue;

            var offset = buildingBlock.transform.position - transform.position;
            var offsetInt = new Vector3Int(
                (int) (offset.x / Game.UnitDistance),
                (int) (offset.y / Game.UnitDistance),
                (int) (offset.z / Game.UnitDistance));

            var address = new Vector3Int(
                offsetInt.x + ScanRange,
                offsetInt.y,
                offsetInt.z + ScanRange);

            array[address.x, address.y, address.z] = buildingBlock;
        }

        return array;
    }

    BuildingBlock[,,] TrimBlockArray(BuildingBlock[,,] blockArray, out Vector3Int min)
    {
        min = new Vector3Int();
        min.x = int.MaxValue;
        min.y = int.MaxValue;
        min.z = int.MaxValue;

        var max = new Vector3Int();

        for (int x = 0; x < blockArray.GetLength(0); x++)
        for (int y = 0; y < blockArray.GetLength(1); y++)
        for (int z = 0; z < blockArray.GetLength(2); z++)
        {
            if (blockArray[x, y, z] == null)
                continue;

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
        
        var final = new BuildingBlock[
            max.x - min.x + 1,
            max.y + 1,
            max.z - min.z + 1];

        for (int x = 0; x < final.GetLength(0); x++)
        for (int y = 0; y < final.GetLength(1); y++)
        for (int z = 0; z < final.GetLength(2); z++)
            final[x, y, z] = blockArray[min.x + x, min.y + y, min.z + z];

        return final;
    }
}