using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cashew.Utility.Extensions;
using DigitalRuby.Tween;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

class PipeBlock : MonoBehaviour
{
    [Header("Config")]
    public int VerticalRange = 10;
    public int HorizontalRange = 10;
    public int MinimumHorizontalRange = 2;
    public float RadiusInWorldUnits = 1;
    public int MaximumConnections = 2;

    [Space]
    public PickUp PickUp;
    public PipeBlockConnection PipeBlockConnectionPrototype;
    public LayerMask ConstructionBlockLayer;
    public Block Block;

    [Space]
    public GameObject StraightPipe;
    public GameObject CornerPipe;
    public GameObject NoPipe;

    public Dictionary<CompassDirection, PipeBlockConnection> Connections;

    [ShowInInspector]
    public bool IsConnected => (Connections?.Count ?? 0) > 0;
    public bool North => Connections.ContainsKey(CompassDirection.North);
    public bool East => Connections.ContainsKey(CompassDirection.East);
    public bool South => Connections.ContainsKey(CompassDirection.South);
    public bool West => Connections.ContainsKey(CompassDirection.West);
    public Vector3 MidPoint => transform.position.WithNewY(y => y + Game.UnitDistance);

    void Start()
    {
        if (transform.parent != null && !PickUp.IsPlacing)
        {
            if (!PickUp.IsPlaced && !PickUp.IsLocked)
                Debug.LogWarning($"{name} is a pipe block that has a parent, but the pick up is not placed or locked.");
            else if (!PickUp.IsPlaced)
                Debug.LogWarning($"{name} is a pipe block that has a parent, but the pick up is not placed.");
            else if (!PickUp.IsLocked)
                Debug.LogWarning($"{name} is a pipe block that has a parent, but the pick up is not locked.");

            if (!PickUp.RB.isKinematic)
                Debug.LogWarning($"{name} is a pipe block that has a parent, but its RB is not set to kinematic.");
        }

        Connections = new Dictionary<CompassDirection, PipeBlockConnection>();
        PickUp.PickedUp += OnPickedUp;
        PickUp.Placed += OnPlaced;
        if (PickUp.IsPlaced && !PickUp.IsOnPallet)
            CheckForConnections();
    }

    void OnPlaced(object sender, EventArgs e)
    {
        if (PickUp.IsOnPallet)
            return;

        CheckForConnections();
        UpdatePipeType();
    }

    void OnPickedUp(object sender, EventArgs e)
    {
        foreach (var kvp in Connections)
            ClearConnection(kvp.Key, kvp.Value);

        Connections.Clear();
        UpdatePipeType();

        //PickUp.Collider = _originalPickUpCollider;
    }

    public void ClearConnection(CompassDirection side, PipeBlockConnection connection)
    {
        if (connection.A == this)
        {
            connection.B.Connections.Remove(Compass.GetOpposingDirection(side));
            connection.B.UpdatePipeType();
        }
        else
        {
            connection.A.Connections.Remove(Compass.GetOpposingDirection(side));
            connection.A.UpdatePipeType();
        }

        Destroy(connection.gameObject);
        UpdatePipeType();
    }

    public virtual void UpdatePipeType()
    {
        StraightPipe.SetActive(false);
        CornerPipe.SetActive(false);
        NoPipe.SetActive(false);

        if (Connections.Count == 1)
        {
            StraightPipe.SetActive(true);

            if (West || East)
                StraightPipe.transform.localRotation = Quaternion.Euler(0, 90, 0);
            else
                StraightPipe.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (North && South)
        {
            StraightPipe.SetActive(true);
            StraightPipe.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //PickUp.Collider = StraightPipe.GetComponentInChildren<Collider>();
        }
        else if (West && East)
        {
            StraightPipe.SetActive(true);
            StraightPipe.transform.localRotation = Quaternion.Euler(0, 90, 0);
            //PickUp.Collider = StraightPipe.GetComponentInChildren<Collider>();
        }
        else if (West && North)
        {
            CornerPipe.SetActive(true);
            CornerPipe.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //PickUp.Collider = CornerPipe.GetComponentInChildren<Collider>();
        }
        else if (North && East)
        {
            CornerPipe.SetActive(true);
            CornerPipe.transform.localRotation = Quaternion.Euler(0, 90, 0);
            //PickUp.Collider = CornerPipe.GetComponentInChildren<Collider>();
        }
        else if (East && South)
        {
            CornerPipe.SetActive(true);
            CornerPipe.transform.localRotation = Quaternion.Euler(0, 180, 0);
            //PickUp.Collider = CornerPipe.GetComponentInChildren<Collider>();
        }
        else if (South && West)
        {
            CornerPipe.SetActive(true);
            CornerPipe.transform.localRotation = Quaternion.Euler(0, -90, 0);
            //PickUp.Collider = CornerPipe.GetComponentInChildren<Collider>();
        }
        else
        {
            NoPipe.SetActive(true);
            //PickUp.Collider = NoPipe.GetComponentInChildren<Collider>();
        }
    }
    
    void CheckForConnections()
    {
        ProcessDirection(CompassDirection.North);
        ProcessDirection(CompassDirection.East);
        ProcessDirection(CompassDirection.South);
        ProcessDirection(CompassDirection.West);
    }

    bool CanBeAttachedTo(CompassDirection side)
    {
        return !Connections.ContainsKey(side) &&
               Connections.Count < MaximumConnections &&
               PickUp.IsPlaced;
    }

    void ProcessDirection(CompassDirection direction)
    {
        var block = CheckAndValidate(direction);
        if (block == null)
            return;

        var connection = Instantiate(PipeBlockConnectionPrototype, transform);
        connection.A = this;
        connection.B = block;
        Connections.Add(direction, connection);
        block.Connections.Add(Compass.GetOpposingDirection(direction), connection);
        block.UpdatePipeType();

        connection.TurnOnLineRenderer();
        connection.CreatePipe();
    }

    PipeBlock CheckAndValidate(CompassDirection direction)
    {
        var pipeBlock = GetPipeBlock(direction);

        if (pipeBlock == null)
            return null;

        if (!pipeBlock.PickUp.IsPlaced || pipeBlock.PickUp.IsOnPallet)
            return null;

        var horizontalDistance = (int) (Mathf.Abs(transform.position.x - pipeBlock.transform.position.x) /
                                        Game.UnitDistance);
        if (horizontalDistance <= 0)
            horizontalDistance = (int) (Mathf.Abs(transform.position.z - pipeBlock.transform.position.z) /
                                        Game.UnitDistance);

        if (horizontalDistance <= 0 || horizontalDistance < MinimumHorizontalRange)
            return null;

        var verticalDistance = Mathf.Abs(transform.position.y - pipeBlock.transform.position.y);
        if (verticalDistance > horizontalDistance - MinimumHorizontalRange - 1)
            return null;
        
        if (!pipeBlock.CanBeAttachedTo(Compass.GetOpposingDirection(direction)))
            return null;

        return pipeBlock;
    }

    PipeBlock GetPipeBlock(CompassDirection direction)
    {
        var pipe = CheckGroundBlockNetwork(direction);
        if (pipe == null)
            pipe = CheckDirectionDirectly(direction);
        return pipe;
    }

    // this simply shoots a ray from this pipe block in the direction and checks only
    // if what it hits is a pipeblock
    PipeBlock CheckDirectionDirectly(CompassDirection direction)
    {
        var ray = new Ray(
            MidPoint,
            Compass.CompassDirectionToVector(direction));

        if (Physics.Raycast(ray, out var hit, HorizontalRange, ConstructionBlockLayer))
        {
            var pipeBlock = hit.transform.GetComponentAnywhere<PipeBlock>();
            if (pipeBlock != null)
                return pipeBlock;
        }

        return null;
    }

    // this tunnels down to the ground via any construction blocks supporting this pipe block,
    // then travels out from there, looking for a construction block
    // if it finds one, it tunnels back up to the top of that stack, checking for a pipe block at the top
    PipeBlock CheckGroundBlockNetwork(CompassDirection direction)
    {
        var baseBlock = Block.GetBaseBlock();
        var ray = new Ray(
            baseBlock.transform.position.WithNewY(y => y + Game.UnitDistance / 2),
            Compass.CompassDirectionToVector(direction));
        if (Physics.Raycast(ray, out var hit, HorizontalRange, ConstructionBlockLayer))
        {
            var pipeBlock = hit.transform.GetComponentAnywhere<PipeBlock>();
            if (pipeBlock != null)
                return pipeBlock;

            var constructionBlock = hit.transform.GetComponentAnywhere<Block>();
            if (constructionBlock != null)
            {
                var topBlock = constructionBlock.GetTopBlock();
                var pBlock = topBlock.GetComponentAnywhere<PipeBlock>();
                if (pBlock != null)
                    return pBlock;
            }
        }

        return null;
    }

    void Reset()
    {
        ConstructionBlockLayer = LayerMask.GetMask("Construction Blocks");
        Block = GetComponent<Block>();
        PickUp = GetComponent<PickUp>();

        StraightPipe = transform.Find("straight")?.gameObject;
        if (StraightPipe == null)
        {
            StraightPipe = new GameObject("straight");
            StraightPipe.transform.parent = transform;
        }

        CornerPipe = transform.Find("corner")?.gameObject;
        if (CornerPipe == null)
        {
            CornerPipe = new GameObject("corner");
            CornerPipe.transform.parent = transform;
        }

        NoPipe = transform.Find("disconnected")?.gameObject;
        if (NoPipe == null)
        {
            NoPipe = new GameObject("disconnected");
            NoPipe.transform.parent = transform;
        }
    }
}

static class Compass
{
    [DebuggerStepThrough]
    public static Vector3 CompassDirectionToVector(CompassDirection direction)
    {
        switch (direction)
        {
            case CompassDirection.North:
                return new Vector3(0, 0, 1);
            case CompassDirection.East:
                return new Vector3(1, 0, 0);
            case CompassDirection.South:
                return new Vector3(0, 0, -1);
            case CompassDirection.West:
                return new Vector3(-1, 0, 0);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    [DebuggerStepThrough]
    public static CompassDirection GetOpposingDirection(CompassDirection direction)
    {
        switch (direction)
        {
            case CompassDirection.North:
                return CompassDirection.South;
            case CompassDirection.East:
                return CompassDirection.West;
            case CompassDirection.South:
                return CompassDirection.North;
            case CompassDirection.West:
                return CompassDirection.East;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}