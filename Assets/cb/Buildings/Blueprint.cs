using System;
using System.Collections.Generic;
using System.Linq;
using Cashew.Utility.Extensions;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;


class Blueprint : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    public BlueprintPackage _package;

    public BlueprintCell[,,] CellArray;
    public List<BlueprintCell> Cells;
    public Vector3Int Dimensions;
    [FormerlySerializedAs("KeyBlockLocation")]
    public Vector3Int KeyBlockPosition;

    [Button]
    public void InitializeArray()
    {
        for (int x = 0; x < Dimensions.x; x++)
        for (int y = 0; y < Dimensions.y; y++)
        for (int z = 0; z < Dimensions.z; z++)
        {
            var cell = Cells.FirstOrDefault(p => p.Location == new Vector3Int(x, y, z));
            if (cell != null)
                CellArray[x, y, z] = cell;
            else
                CellArray[x, y, z] = new BlueprintCell()
                {
                    IsEmpty = true,
                    Location = new Vector3Int(x, y, z)
                };
        }
    }

    public Vector3 GetHologramPosition(BlueprintInstance instance)
    {
        var offset = instance.Building.BuildingHologramOffsetFromKeyBlock;

        for (int i = 0; i < instance.Rotations; i++)
            offset = Rotate(offset);

        return instance.KeyBlock.NetworkBlock.Block.TargetPosition -
               new Vector3(Game.HalfUnitDistance, 0, Game.HalfUnitDistance) +
               offset;
    }

    public bool MatchesBlockArray(NetworkBlock[,,] blockArray, out int rotations, out BuildingBlock keyBlock)
    {
        for (int i = 0; i < 4; i++)
        {
            rotations = i;
            if (MatchesBlockArrayImpl(blockArray))
            {
                keyBlock = blockArray[KeyBlockPosition.x, KeyBlockPosition.y, KeyBlockPosition.z]
                    .GetComponentAnywhere<BuildingBlock>();
                return true;
            }

            blockArray = Rotate(blockArray);
        }

        rotations = 0;
        keyBlock = null;
        return false;
    }

    bool MatchesBlockArrayImpl(NetworkBlock[,,] blockArray)
    {
        if (blockArray.GetLength(0) != Dimensions.x ||
            blockArray.GetLength(1) != Dimensions.y ||
            blockArray.GetLength(2) != Dimensions.z)
            return false;

        for (int x = 0; x < Dimensions.x; x++)
        for (int y = 0; y < Dimensions.y; y++)
        for (int z = 0; z < Dimensions.z; z++)
        {
            BuildingBlock block = null;
            if (blockArray[x, y, z] != null)
                block = blockArray[x, y, z].GetComponentAnywhere<BuildingBlock>();

            if (CellArray[x, y, z].IsEmpty && block == null)
                continue;

            if (CellArray[x, y, z].IsEmpty && block != null)
                return false;

            if (!CellArray[x, y, z].IsEmpty && block == null)
                return false;

            if (!CellArray[x, y, z].IsEmpty && block != null &&
                CellArray[x, y, z].BuildingBlockColorRequired != block.Color)
                return false;
        }

        return true;
    }

    Vector3 Rotate(Vector3 pos)
    {
        // top right -> bottom left
        if (pos.x >= 0 && pos.z >= 0)
        {
            return new Vector3(
                Mathf.Abs(pos.z), 
                pos.y, 
                -Mathf.Abs(pos.x));
        }
        // bottom right -> bottom left
        if (pos.x >= 0 && pos.z <= 0)
        {
            return new Vector3(
                -Mathf.Abs(pos.z),
                pos.y, 
                -Mathf.Abs(pos.x));
        }
        // bottom left -> top left
        if (pos.x <= 0 && pos.z <= 0)
        {
            return new Vector3(
                -Mathf.Abs(pos.z),
                pos.y,
                Mathf.Abs(pos.x));
        }
        // top left -> top right
        if (pos.x <= 0 && pos.y >= 0)
        {
            return new Vector3(
                Mathf.Abs(pos.z),
                pos.y,
                Mathf.Abs(pos.x));
        }

        throw new InvalidOperationException();
    }

    Vector3Int RotatePosition(Vector3Int pos, Vector3Int originalSize)
    {
        return new Vector3Int(pos.z, pos.y, originalSize.x - pos.x - 1);
    }

    static NetworkBlock[,,] Rotate(NetworkBlock[,,] array)
    {
        var rot = new NetworkBlock[array.GetLength(2), array.GetLength(1), array.GetLength(0)];

        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                for (int z = 0; z < array.GetLength(2); z++)
                {
                    rot[z, y, x] = array[x, y, array.GetLength(2) - z - 1];
                }
            }
        }

        return rot;
    }

    public static Blueprint FromBlockArray(BuildingBlock[,,] blockArray, Vector3Int keyBlockLocation)
    {
        var bp = (Blueprint) CreateInstance(typeof(Blueprint));
        bp.Dimensions = new Vector3Int(
            blockArray.GetLength(0),
            blockArray.GetLength(1),
            blockArray.GetLength(2));
        bp.KeyBlockPosition = keyBlockLocation;
        bp.CellArray = new BlueprintCell[bp.Dimensions.x, bp.Dimensions.y, bp.Dimensions.z];
        bp.Cells = new List<BlueprintCell>();

        for (int x = 0; x < blockArray.GetLength(0); x++)
        for (int y = 0; y < blockArray.GetLength(1); y++)
        for (int z = 0; z < blockArray.GetLength(2); z++)
        {
            var b = blockArray[x, y, z];
            if (b == null)
                bp.CellArray[x, y, z] = BlueprintCell.Empty(new Vector3Int(x, y, z));
            else
            {
                bp.CellArray[x, y, z] = BlueprintCell.Occupied(new Vector3Int(x, y, z), b.Color);
                bp.Cells.Add(bp.CellArray[x, y, z]);
            }
        }

        return bp;
    }

    public void OnBeforeSerialize()
    {
        _package = BlueprintPackage.Serialize(CellArray);
    }

    public void OnAfterDeserialize()
    {
        CellArray = _package.Deserialize();
    }

    // used to support multi-dim array serialization
    [Serializable]
    public class BlueprintPackage
    {
        public Vector3Int Dimensions;
        public List<BlueprintCellPackage> Cells;

        public BlueprintCell[,,] Deserialize()
        {
            var cells = new BlueprintCell[Dimensions.x, Dimensions.y, Dimensions.z];

            foreach (var cell in Cells)
                cells[cell.Location.x, cell.Location.y, cell.Location.z] = cell.Cell;

            return cells;
        }

        public static BlueprintPackage Serialize(BlueprintCell[,,] cells)
        {
            var package = new BlueprintPackage();
            package.Dimensions = new Vector3Int(
                cells.GetLength(0),
                cells.GetLength(1),
                cells.GetLength(2));
            package.Cells = new List<BlueprintCellPackage>();

            for (int x = 0; x < cells.GetLength(0); x++)
            for (int y = 0; y < cells.GetLength(1); y++)
            for (int z = 0; z < cells.GetLength(2); z++)
                package.Cells.Add(
                    new BlueprintCellPackage()
                    {
                        Location = new Vector3Int(x, y, z),
                        Cell = cells[x, y, z]
                    });

            return package;
        }
    }
    
    [Serializable]
    public class BlueprintCellPackage
    {
        public Vector3Int Location;
        public BlueprintCell Cell;
    }
}