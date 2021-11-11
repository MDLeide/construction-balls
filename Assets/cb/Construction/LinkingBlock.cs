using Cashew.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


class LinkingBlock : MonoBehaviour
{
    [Header("Config")]
    // this is used to find linking block candidates
    public LayerMask LinkingBlockLayerMask;

    // this is used when checking for obstructions while linking
    public LayerMask ObstructionLayerMask;
    // this is used to specific types of linking blocks. ie pipe blocks vs tube blocks
    public int LinkingBlockLayer;
    public PickUp PickUp;
    public Block Block;
    public Link LinkPrototype;

    [Header("Settings")]
    //public int VerticalRange = 10;
    public int HorizontalRange = 10;

    [Space]
    [Tooltip("When making a link, the partner linking block must be at least this far away. -1 for no minimum. Measured in block units.")]
    public int MinimumHorizontalDistance = 2;
    [Tooltip("When making a link, the partner linking block must be no greater than this far away. -1 for no minimum. Measured in block units.")]
    public int MaximumHorizontalDistance = 10;

    [Space]
    public int MinimumVerticalDistance = 0;

    public int MaximumVerticalDistance = 3;
    
    [Tooltip("This is the maximum ratio VerticalDistance:HorizontalDistance that is allowed")]
    public float MaximumClimbRatio = 1;
    [Space]
    public float RadiusInWorldUnits = 1;
    [Space]
    public int MaxLinks = 2;

    [Button]
    public void AnyDirection()
    {
        AllowedDirections = new List<CompassDirection>
        {
            CompassDirection.North,
            CompassDirection.South,
            CompassDirection.East,
            CompassDirection.West
        };
    }

    public List<CompassDirection> AllowedDirections = new List<CompassDirection>
    {
        CompassDirection.North,
        CompassDirection.South,
        CompassDirection.East,
        CompassDirection.West
    };

    public Func<LinkingBlock, bool> CanLinkTo = l => true;
    public Dictionary<CompassDirection, Link> Links;

    public bool IsLinked => Links.Any();

    // only the newly placed block raises this event. you can get the other block
    // through the link
    public event EventHandler<LinkEventArgs> LinkMade;

    // raised by the recipient of a new link
    public event EventHandler<LinkEventArgs> LinkReceived;

    // both sides of the link raise this event
    public event EventHandler<LinkEventArgs> LinkBroken; 
    

    void Start()
    {
        Links = new Dictionary<CompassDirection, Link>();

        if (PickUp == null)
            return;

        if (PickUp.IsPlaced && !PickUp.IsOnPallet)
            FindLinks();

        PickUp.Placed += OnPlaced;
        PickUp.PickedUp += OnPickedUp;

        Block.StabilityLost += BlockOnStabilityLost;
    }

    void BlockOnStabilityLost(object sender, EventArgs e)
    {
        var copy = Links.ToDictionary(p => p.Key, p => p.Value);
        Links.Clear();

        foreach (var kvp in copy)
        {
            LinkBroken?.Invoke(this, new LinkEventArgs(kvp.Value));
            kvp.Value.OnLinkBroken();

            kvp.Value.Other(this).OnLinkBroken(Compass.GetOpposingDirection(kvp.Key), kvp.Value);

            Destroy(kvp.Value.gameObject);
        }

        Links.Clear();
    }

    void OnPickedUp(object sender, EventArgs e)
    {
        var copy = Links.ToDictionary(p => p.Key, p => p.Value);
        Links.Clear();

        foreach (var kvp in copy)
        {
            LinkBroken?.Invoke(this, new LinkEventArgs(kvp.Value));
            kvp.Value.OnLinkBroken();

            kvp.Value.Other(this).OnLinkBroken(Compass.GetOpposingDirection(kvp.Key), kvp.Value);

            Destroy(kvp.Value.gameObject);
        }

        Links.Clear();
    }

    void OnLinkBroken(CompassDirection direction, Link link)
    {
        Links.Remove(direction);
        LinkBroken?.Invoke(this, new LinkEventArgs(link));
        link.OnLinkBroken();
    }

    void OnPlaced(object sender, EventArgs e)
    {
        if (PickUp.IsOnPallet)
            return;
        FindLinks();
    }

    public bool HasLink(CompassDirection direction) => Links.ContainsKey(direction);
    
    public bool CanBeLinkedTo(CompassDirection side)
    {
        return AllowedDirections.Contains(GlobalToLocal(side)) &&
               !Links.ContainsKey(side) &&
               Links.Count < MaxLinks &&
               PickUp.IsPlaced;
    }

    CompassDirection GlobalToLocal(CompassDirection global)
    {
        if (transform.forward == Vector3.forward)
            return global;
        if (transform.forward == Vector3.right)
            return Compass.RotateClockwise(global);
        if (transform.forward == Vector3.left)
            return Compass.RotateCounterClockwise(global);
        if (transform.forward == Vector3.back)
            return Compass.GetOpposingDirection(global);

        throw new InvalidOperationException();
    }

    void FindLinks()
    {
        var hits = Physics.OverlapBox(
            transform.position,
            Vector3.one * HorizontalRange,
            Quaternion.identity,
            LinkingBlockLayerMask);

        var blocks = new List<LinkingBlock>();

        foreach (var hit in hits)
        {
            var linkingBlock = hit.transform.GetComponentAnywhere<LinkingBlock>();
            if (linkingBlock == null)
                continue;

            if (linkingBlock == this)
                continue;

            if (!blocks.Contains(linkingBlock))
                blocks.Add(linkingBlock);
        }

        var toLink = new List<System.Tuple<CompassDirection, LinkingBlock>>();

        foreach (var linkingBlock in blocks)
            if (CanLinkToBlock(linkingBlock, out var direction))
                toLink.Add(new System.Tuple<CompassDirection, LinkingBlock>(direction, linkingBlock));

        foreach (var tuple in toLink)
            LinkToBlock(tuple.Item2, tuple.Item1);
    }

    void LinkToBlock(LinkingBlock block, CompassDirection direction)
    {
        var link = Instantiate(LinkPrototype, transform);
        link.Creator = this;
        link.OtherBlock = block;
        Links.Add(direction, link);
        block.Links.Add(Compass.GetOpposingDirection(direction), link);

        LinkMade?.Invoke(this, new LinkEventArgs(link));
        link.OtherBlock.LinkReceived?.Invoke(this, new LinkEventArgs(link));
    }

    bool CanLinkToBlock(LinkingBlock block, out CompassDirection direction)
    {
        direction = CompassDirection.Up;

        if (block == null || !block.enabled)
            return false;

        if (!block.PickUp.IsPlaced || block.PickUp.IsOnPallet)
            return false;

        if (!CanLinkTo(block))
            return false;

        var verticalDist = (int)(Mathf.Abs(block.transform.position.y - transform.position.y) / Game.UnitDistance);
        var horizontalDist = GetHorizontalDistance(block, out direction);

        if (horizontalDist < 0)
            return false;

        if (Links.ContainsKey(direction))
            return false;

        if (!AllowedDirections.Contains(direction))
            return false;

        if (MinimumHorizontalDistance > 0 && horizontalDist < MinimumHorizontalDistance)
            return false;

        if (MaximumHorizontalDistance > 0 && horizontalDist > MaximumHorizontalDistance)
            return false;

        if (MinimumVerticalDistance > 0 && verticalDist < MinimumVerticalDistance)
            return false;

        if (MaximumVerticalDistance > 0 && verticalDist > MaximumVerticalDistance)
            return false;

        var climb = verticalDist / horizontalDist;
        if (climb > MaximumClimbRatio)
            return false;

        if (!block.CanBeLinkedTo(Compass.GetOpposingDirection(direction)))
            return false;


        // cast a ray from this block to the target
        // if we don't hit anything, or if we hit the target itself
        // we'll assume the space is clear
        // otherwise, we are obstructed
        var distance = Mathf.Abs((block.transform.position - transform.position).magnitude);
        var ray = new Ray(Block.MidPoint, (block.Block.MidPoint - Block.MidPoint).normalized);
        if (Physics.Raycast(ray, out var hit, distance, ObstructionLayerMask))
        {
            var linkingBlock = hit.transform.GetComponentAnywhere<LinkingBlock>();
            return linkingBlock == block;
        }

        return true;
    }

    int GetHorizontalDistance(LinkingBlock block, out CompassDirection direction)
    {
        if (Mathf.Abs(block.transform.position.x - transform.position.x) <= float.Epsilon)
        {
            if (block.transform.position.z > transform.position.z)
                direction = CompassDirection.North;
            else if (block.transform.position.z < transform.position.z)
                direction = CompassDirection.South;
            else if (block.transform.position.y > transform.position.y)
                direction = CompassDirection.Up;
            else if (block.transform.position.y < transform.position.y)
                direction = CompassDirection.Down;
            else
                throw new InvalidOperationException();

            return (int)(Mathf.Abs(block.transform.position.z - transform.position.z) / Game.UnitDistance);
        }
        else if (Mathf.Abs(block.transform.position.z - transform.position.z) <= float.Epsilon)
        {
            if (block.transform.position.x > transform.position.x)
                direction = CompassDirection.East;
            else if (block.transform.position.x < transform.position.x)
                direction = CompassDirection.West;
            else
                throw new InvalidOperationException();

            return (int)(Mathf.Abs(block.transform.position.x - transform.position.x) / Game.UnitDistance);
        }
        else
        {
            direction = CompassDirection.Up;
            return -1;
        }
    }
    
    void Reset()
    {
        LinkingBlockLayerMask = LayerMask.GetMask("Construction Blocks");

        Block = GetComponent<Block>();
        PickUp = GetComponent<PickUp>();

        LinkPrototype = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/cb/Construction/link.prefab")?.GetComponent<Link>();
    }
}