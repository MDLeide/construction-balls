using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

class Block : MonoBehaviour
{
    int _checkStability;

    [Header("Attached Blocks"), ReadOnly]
    public Dictionary<CompassDirection, Block> Neighbors = new Dictionary<CompassDirection, Block>();
    public List<Block> NeighborList = new List<Block>();

    [Header("Settings")]
    public PickUp PickUp;
    public InputActionReference BuildOnTopAction;
    public LayerMask BlockMask;
    public LayerMask ObstructionMask;
    public TMP_Text IdText;
    [Tooltip("If true, the Unstable and Stable gameobjects will be used to indicate stability. They will be turned on/off.")]
    public bool ShowStableGraphics = true;

    [Space]
    public GameObject Unstable;
    public GameObject Stable;

    [Header("State")]
    public Vector3 TargetPosition;
    public bool IsGrounded;

    public Vector3 MidPoint => transform.position + transform.up * Game.UnitDistance / 2;

    public Block OnTop
    {
        get
        {
            if (Neighbors.ContainsKey(CompassDirection.Up))
            {
                if (Neighbors[CompassDirection.Up] == this)
                    throw new InvalidOperationException();

                return Neighbors[CompassDirection.Up];
            }
            return null;
        }
    }

    //
    // Events
    //

    // raised whenever neighbors is updated
    public event EventHandler StabilityLost;
    public event EventHandler PreviewNeighborsCleared;
    public event EventHandler NeighborsUpdated;

    void Start()
    {
        PickUp.PickedUp += PickUpOnPickedUp;
        PickUp.BeingPlaced += PickUpOnBeingPlaced;
    }

    void Update()
    {
        if (_checkStability == Time.frameCount)
            if (!IsStable())
                LostStability();
    }

    void PickUpOnPickedUp(object sender, EventArgs e)
    {
        if (ShowStableGraphics)
        {
            Stable.SetActive(false);
            Unstable.SetActive(true);
        }

        foreach (var kvp in Neighbors)
        {
            kvp.Value._checkStability = Time.frameCount + 1;
            kvp.Value.Neighbors.Remove(Compass.GetOpposingDirection(kvp.Key));
            kvp.Value.NeighborList.Remove(this);
        }
        
        // allow subscribers to see the neighbors before they are changed
        PreviewNeighborsCleared?.Invoke(this, new EventArgs());
        Neighbors.Clear();
        NeighborList.Clear();
        IsGrounded = false;
    }

    void PickUpOnBeingPlaced(object sender, PickUpPlacingEventArgs e)
    {
        if (ShowStableGraphics)
        {
            Stable.SetActive(true);
            Unstable.SetActive(false);
        }

        TargetPosition = e.TargetPosition;
    }

    #region stability

    /// <summary>
    /// Determines if this block would still be attached to the ground
    /// if the <paramref name="blockBeingRemoved"/> was removed.
    /// </summary>
    /// <param name="blockBeingRemoved"></param>
    /// <returns></returns>
    bool IsGroundedIndependentOf(Block blockBeingRemoved, List<Block> blocksChecked = null)
    {
        if (blocksChecked == null)
            blocksChecked = new List<Block>();

        if (IsGrounded)
            return true;

        foreach (var kvp in Neighbors)
        {
            if (kvp.Value != blockBeingRemoved &&
                !blocksChecked.Contains(kvp.Value))
            {
                blocksChecked.Add(kvp.Value); 
                if (kvp.Value.IsGroundedIndependentOf(this, blocksChecked))
                    return true;
            }
        }

        return false;
    }

    bool IsStable()
    {
        if (IsGrounded)
            return true;

        foreach (var attached in Neighbors)
            if (attached.Value.IsGroundedIndependentOf(this))
                return true;

        return false;
    }

    void LostStability()
    {
        if (ShowStableGraphics)
        {
            Stable.SetActive(false);
            Unstable.SetActive(true);
        }

        foreach (var kvp in Neighbors)
        {
            kvp.Value._checkStability = Time.frameCount + 1;
            kvp.Value.Neighbors.Remove(Compass.GetOpposingDirection(kvp.Key));
            kvp.Value.NeighborList.Remove(this);
        }

        Neighbors.Clear();
        NeighborList.Clear();
        PickUp.Release();
        StabilityLost?.Invoke(this, new EventArgs());
    }

    #endregion

    #region Place

    bool CanPlaceOnTop()
    {
        if (OnTop != null)
            return OnTop.CanPlaceOnTop();

        return !Physics.BoxCast(
            TargetPosition,
            Game.HalfUnitCube,
            Vector3.up,
            Quaternion.identity,
            Game.SmallUnitDistance,
            ObstructionMask);
    }

    public bool CanPlace(RaycastHit hit)
    {
        // could turn this component on only when the pickup is placed... consideration
        if (!PickUp.IsPlaced || PickUp.IsOnPallet || PickUp.IsPlacing)
            return false;

        if (BuildOnTopAction.action.ReadValue<float>() > 0)
            return CanPlaceOnTop();

        var direction = hit.normal;
        
        if (Neighbors.ContainsKey(Compass.VectorToCompassDirection(direction)))
            return false;

        return !Physics.BoxCast(
            MidPoint,
            Game.SmallHalfUnitCube,
            direction,
            Quaternion.identity,
            Game.UnitDistance,
            ObstructionMask);
    }

    public void PlaceOnTop(Block blockToBePlaced)
    {
        blockToBePlaced.transform.parent = null;
        blockToBePlaced.transform.rotation = Quaternion.identity;

        if (OnTop == null)
            PlaceOn(blockToBePlaced, Vector3.up);
        else
            OnTop.PlaceOnTop(blockToBePlaced);
    }

    public void PlaceOn(Block blockToBePlaced, RaycastHit hit)
    {
        if (BuildOnTopAction.action.ReadValue<float>() > 0)
            PlaceOnTop(blockToBePlaced);
        else
            PlaceOn(blockToBePlaced, hit.normal);
    }

    void PlaceOn(Block blockToBePlaced, Vector3 direction)
    {
        if (blockToBePlaced == this)
            throw new InvalidOperationException();

        var side = Compass.VectorToCompassDirection(direction);

        Neighbors.Add(side, blockToBePlaced);
        blockToBePlaced.Neighbors.Add(Compass.GetOpposingDirection(side), this);
        
        blockToBePlaced.transform.parent = null;
        blockToBePlaced.transform.rotation = Quaternion.identity;

        // local position is used here to support cases where the target is moving, such as when placing on a pallet
        var target = TargetPosition + direction * Game.UnitDistance;

        blockToBePlaced.PickUp.Place(target);

        blockToBePlaced.CheckForNeighbors();
        blockToBePlaced.CheckForGrounded(target);

        //blockToBePlaced.RaisePlaced();
    }

    public void RaisePlaced()
    {
        // Placed?.Invoke(this, new EventArgs());
    }

    void CheckForGrounded(Vector3 targetPosition)
    {
        var ray = new Ray(targetPosition + Vector3.up * .1f, Vector3.down);
        if (Physics.Raycast(ray, out var hit, .2f))
        {
            var panel = hit.transform.GetComponentAnywhere<Panel>();
            if (panel != null)
                IsGrounded = true;
        }
    }

    public void CheckForNeighbors()
    {
        CheckForNeighbor(CompassDirection.Up);
        CheckForNeighbor(CompassDirection.Down);
        CheckForNeighbor(CompassDirection.North);
        CheckForNeighbor(CompassDirection.East);
        CheckForNeighbor(CompassDirection.South);
        CheckForNeighbor(CompassDirection.West);

        NeighborsUpdated?.Invoke(this, new EventArgs());
    }

    void CheckForNeighbor(CompassDirection direction)
    {
        if (Neighbors.ContainsKey(direction))
            return;

        var neighbor = GetNeighbor(direction);
        if (neighbor == null)
            return;

        Neighbors.Add(direction, neighbor);
        NeighborList.Add(neighbor);
        neighbor.Neighbors.Add(Compass.GetOpposingDirection(direction), this);
        neighbor.NeighborList.Add(this);
    }

    Block GetNeighbor(CompassDirection direction)
    {
        var dir = Compass.CompassDirectionToVector(direction);
        var ray = new Ray(TargetPosition + Vector3.up * Game.UnitDistance / 2, dir);
        if (Physics.Raycast(ray, out var hit, Game.UnitDistance, BlockMask))
        {
            var block = hit.transform.GetComponentAnywhere<Block>();
            if (block != null && block.PickUp.IsPlaced)
                return block;
        }

        return null;
    }

#endregion

    #region find blocks

    public bool IsConnectedTo(Block block)
    {
        return IsConnectedTo(block, new List<Block> {this});
    }

    bool IsConnectedTo(Block block, List<Block> checkedBlocks)
    {
        if (block == this)
            return true;

        foreach (var neighbor in Neighbors)
        {
            if (neighbor.Value == block)
                return true;

            if (!checkedBlocks.Contains(neighbor.Value))
            {
                checkedBlocks.Add(neighbor.Value);
                if (neighbor.Value.IsConnectedTo(block, checkedBlocks))
                    return true;
            }
        }

        return false;
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

    #endregion

    void Reset()
    {
        PickUp = GetComponent<PickUp>();
        if (PickUp == null)
            PickUp = gameObject.AddComponent<PickUp>();
    }
}