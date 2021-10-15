using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class Panel : MonoBehaviour
{
    readonly List<Block> _placed = new List<Block>();

    float UnitDistanceInstance => Game.UnitDistance;

    public void Place(Block block, RaycastHit hit)
    {
        var xUnits = (int) (hit.point.x / UnitDistanceInstance);
        if (hit.point.x < 0)
            xUnits--;

        var zUnits = (int) (hit.point.z / UnitDistanceInstance);
        if (hit.point.z < 0)
            zUnits--;

        var position = new Vector3(
            xUnits * UnitDistanceInstance + UnitDistanceInstance / 2,
            0,
            zUnits * UnitDistanceInstance + UnitDistanceInstance / 2);

        var existing = _placed.FirstOrDefault(p => p.TargetPosition == position);
        if (existing != null)
        {
            existing.PlaceOnTop(block);
            return;
        }

        block.transform.parent = null;
        block.transform.rotation = Quaternion.identity;
        _placed.Add(block);
        block.PickUp.PickedUp += (sender, args) => _placed.Remove(block);

        block.PickUp.Place(position);
    }
}