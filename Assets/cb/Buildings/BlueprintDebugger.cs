using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

class BlueprintDebugger : MonoBehaviour
{
    public Blueprint Blueprint;

    public GameObject Block;
    public GameObject KeyBlock;

    [Button]
    public void DeployCells()
    {
        var toDestroy = new List<GameObject>();

        foreach (Transform child in transform)
            toDestroy.Add(child.gameObject);

        foreach (var obj in toDestroy)
            DestroyImmediate(obj);

        foreach (var cell in Blueprint.Cells)
        {
            if (cell.IsEmpty)
                continue;

            if (cell.Location == Blueprint.KeyBlockPosition)
            {
                var block = Instantiate(KeyBlock, transform);
                block.transform.localPosition = new Vector3(
                    cell.Location.x * Game.UnitDistance,
                    cell.Location.y * Game.UnitDistance,
                    cell.Location.z * Game.UnitDistance);
            }
            else
            {
                var block = Instantiate(Block, transform);
                block.transform.localPosition = new Vector3(
                    cell.Location.x * Game.UnitDistance,
                    cell.Location.y * Game.UnitDistance,
                    cell.Location.z * Game.UnitDistance);
            }
        }
    }

    [Button]
    public void DeployCellArray()
    {
        var toDestroy = new List<GameObject>();

        foreach (Transform child in transform)
            toDestroy.Add(child.gameObject);

        foreach (var obj in toDestroy)
            DestroyImmediate(obj);

        foreach (var cell in Blueprint.CellArray)
        {
            if (cell.IsEmpty)
                continue;

            if (cell.Location == Blueprint.KeyBlockPosition)
            {
                var block = Instantiate(KeyBlock, transform);
                block.transform.localPosition = new Vector3(
                    cell.Location.x * Game.UnitDistance,
                    cell.Location.y * Game.UnitDistance,
                    cell.Location.z * Game.UnitDistance);
            }
            else
            {
                var block = Instantiate(Block, transform);
                block.transform.localPosition = new Vector3(
                    cell.Location.x * Game.UnitDistance,
                    cell.Location.y * Game.UnitDistance,
                    cell.Location.z * Game.UnitDistance);
            }
        }
    }
}