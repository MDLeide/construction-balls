using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;


class Blueprint : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    public BlueprintPackage _package;

    public BlueprintCell[,,] CellArray;
    public List<BlueprintCell> Cells;
    public Vector3Int Dimensions;
    public Vector3Int KeyBlockLocation;

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

    public bool MatchesBlockArray(BuildingBlock[,,] blockArray)
    {
        if (blockArray.GetLength(0) != Dimensions.x ||
            blockArray.GetLength(1) != Dimensions.y ||
            blockArray.GetLength(2) != Dimensions.z)
            return false;

        for (int x = 0; x < Dimensions.x; x++)
        {
            for (int y = 0; y < Dimensions.y; y++)
            {
                for (int z = 0; z < Dimensions.z; z++)
                {
                    if (CellArray[x, y, z].IsEmpty && blockArray[x, y, z] != null ||
                        !CellArray[x, y, z].IsEmpty &&
                        CellArray[x, y, z].BuildingBlockColorRequired != blockArray[x, y, z].Color)
                        return false;
                }
            }
        }

        return true;
    }

    public static Blueprint FromBlockArray(BuildingBlock[,,] blockArray, Vector3Int keyBlockLocation)
    {
        var bp = (Blueprint) CreateInstance(typeof(Blueprint));
        bp.Dimensions = new Vector3Int(
            blockArray.GetLength(0),
            blockArray.GetLength(1),
            blockArray.GetLength(2));
        bp.KeyBlockLocation = keyBlockLocation;
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