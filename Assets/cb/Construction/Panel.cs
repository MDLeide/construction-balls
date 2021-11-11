using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew;
using Cashew.Utility.Extensions;
using UnityEngine;


class Panel : MonoBehaviour
{
    readonly List<Block> _placed = new List<Block>();

    public LayerMask InterferenceLayerMask;

    public bool CanPlace(RaycastHit hit)
    {
        var check = GridHelper.GetCenterPointOfCell(hit.point);
        var hits = Physics.OverlapBox(check, Game.SmallHalfUnitCube, Quaternion.identity, InterferenceLayerMask);
        
        foreach (var o in hits)
        {
            var panel = o.GetComponentAnywhere<Panel>();
            if (panel == null)
                return false;
        }

        return true;
    }

    public void Place(Block block, RaycastHit hit)
    {
        var position = GridHelper.GetBottomPointOfCell(hit.point);

        var existing = _placed.FirstOrDefault(p => p.TargetPosition == position);
        if (existing != null)
        {
            existing.PlaceOnTop(block);
            return;
        }

        block.IsGrounded = true;
        block.transform.parent = null;
        block.transform.rotation = Quaternion.identity;
        
        _placed.Add(block);
        
        block.PickUp.PickedUp += PickUpOnPickedUp;
        block.PickUp.Placed += PickUpOnPlaced; 

        block.PickUp.Place(position);

        void PickUpOnPlaced(object sender, EventArgs e)
        {
            block.CheckForNeighbors();
            block.PickUp.Placed -= PickUpOnPlaced;
        }

        void PickUpOnPickedUp(object sender, EventArgs e)
        {
            _placed.Remove(block);
            block.PickUp.PickedUp -= PickUpOnPickedUp;
        }

        //block.RaisePlaced();
    }

    
}