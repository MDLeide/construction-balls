using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;


class Block : MonoBehaviour
{
    Dictionary<Vector3, Block> _attached = new Dictionary<Vector3, Block>();

    [ReadOnly]
    public Vector3 TargetPosition;
    // keep a reference to the block placed on top so we can 
    // more reliably build upwards
    [ReadOnly]
    public Block _onTop;

    public PickUp PickUp;
    public InputActionReference BuildOnTopAction;
    public Vector3 MidPoint => transform.position + Vector3.up * Game.UnitDistance / 2;

    void Start()
    {
        PickUp.PickedUp += PickUpOnPickedUp;
        PickUp.BeingPlaced += PickUpOnBeingPlaced;
    }

    void PickUpOnPickedUp(object sender, EventArgs e)
    {
        _onTop = null;
    }

    void PickUpOnBeingPlaced(object sender, PickUpPlacingEventArgs e)
    {
        TargetPosition = e.TargetLocalPosition;
    }

    public void PlaceOn(Block blockToBePlaced, RaycastHit hit)
    {
        if (!PickUp.IsPlaced)
            throw new InvalidOperationException();
        
        if (BuildOnTopAction.action.ReadValue<float>() > 0)
            PlaceOnTop(blockToBePlaced);
        else if (hit.normal == Vector3.up)
            PlaceOnTop(blockToBePlaced);
        else
            PlaceOn(blockToBePlaced, hit.normal);
    }

    public bool CanPlace(RaycastHit hit)
    {
        if (!PickUp.IsPlaced || PickUp.IsOnPallet || PickUp.IsPlacing)
            return false;

        if (BuildOnTopAction.action.ReadValue<float>() > 0)
            return CanPlaceOnTop();
        
        return CanPlace(hit.normal);
    }

    public void PlaceOnTop(Block blockToBePlaced)
    {
        blockToBePlaced.transform.parent = null;
        blockToBePlaced.transform.rotation = Quaternion.identity;

        if (_onTop == null)
        {
            _onTop = blockToBePlaced;
            _onTop.PickUp.Place(TargetPosition + Vector3.up * Game.UnitDistance);
            _onTop.PickUp.PickedUp += (sender, args) => _onTop = null;
        }
        else
        {
            _onTop.PlaceOnTop(blockToBePlaced);
        }
    }

    bool CanPlaceOnTop()
    {
        if (_onTop != null)
            return _onTop.CanPlaceOnTop();

        var position = transform.position;

        while (true)
        {
            var ray = new Ray(position, Vector3.up);
            if (Physics.Raycast(ray, out var hit, Game.UnitDistance))
            {
                var block = hit.transform.GetComponent<Block>();
                if (block == null)
                    return false;
                position += Vector3.up * Game.UnitDistance;
            }
            else
            {
                return true;
            }
        }
    }

    bool CanPlace(Vector3 direction)
    {
        if (_attached.ContainsKey(direction))
            return false;

        var ray = new Ray(MidPoint, direction);
        return !Physics.Raycast(ray, Game.UnitDistance);
    }

    void PlaceOn(Block blockToBePlaced, Vector3 direction)
    {
        _attached.Add(direction, blockToBePlaced);
        blockToBePlaced.PickUp.PickedUp += (sender, args) => _attached.Remove(direction);

        blockToBePlaced.transform.parent = null;
        blockToBePlaced.transform.rotation = Quaternion.identity;

        blockToBePlaced.PickUp.Place(transform.localPosition + direction * Game.UnitDistance);
    }
    
    public Block GetTopBlock()
    {
        var ray = new Ray(MidPoint, Vector3.up);
        if (Physics.Raycast(ray, out var hit, Game.UnitDistance))
        {
            var block = hit.transform.GetComponentAnywhere<Block>();
            if (block == this)
                return this;

            if (block != null)
                return block.GetTopBlock();
        }

        return this;
    }

    public Block GetBaseBlock()
    {
        var ray = new Ray(MidPoint, Vector3.down);
        if (Physics.Raycast(ray, out var hit, Game.UnitDistance))
        {
            var block = hit.transform.GetComponentAnywhere<Block>();
            if (block == this)
                return this;

            if (block != null)
                return block.GetBaseBlock();
        }

        return this;
    }

    void Reset()
    {
        PickUp = GetComponent<PickUp>();
        if (PickUp == null)
            PickUp = gameObject.AddComponent<PickUp>();
    }
}