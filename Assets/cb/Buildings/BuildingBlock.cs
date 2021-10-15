using System.Collections.Generic;
using Cashew.Utility.Extensions;
using TMPro;
using UnityEngine;

class BuildingBlock : MonoBehaviour
{
    const int CompletionFrameDelay = 2;
    const int ScanRange = 50;
    const int ScanHeight = 25;

    int _checkFrame;
    bool _check;
    GameObject _hologram;

    public BlueprintInstance Blueprint;

    public BallColor Color = BallColor.Blue;
    public bool CanBeBuilt;
    public GameObject CanBeBuiltGraphics;
    public GameObject IsKeyGraphics;
    public PickUp PickUp;
    public TMP_Text ReadyToBuildText;
    public Interactable BuildInteractable;
    [Tooltip("How far to search when updating other building blocks in the area after being placed.")]
    public float UpdateRange = 15f;

    void Start()
    {
        Blueprint = null;
        PickUp.Placed += (sender, args) => OnPlaced();
        PickUp.PickedUp += (sender, args) => OnPickedUp();
        BuildInteractable.Pushed += OnBuild;
    }

    void OnBuild(object sender, InteractablePushedEventArgs e)
    {
        if (!CanBeBuilt || Blueprint.KeyBlock != this)
            return;

        Destroy(_hologram);

        var bldg = Instantiate(Blueprint.Building.BuildingPrototype);
        bldg.transform.position = transform.position + Blueprint.Building.BuildingOffsetFromKeyBlock;

        foreach (var block in Blueprint.OtherBlocks)
            Destroy(block.gameObject);

        Destroy(gameObject);
    }

    void Update()
    {
        if (_checkFrame <= Time.frameCount && _check)
        {
            CheckCompletion();
            _check = false;
        }
    }

    void CheckForCompletionNextTick()
    {
        _checkFrame = Time.frameCount + CompletionFrameDelay;
        _check = true;
    }

    public void CheckCompletion()
    {
        if (!PickUp.IsPlaced)
            return;

        if (CanBeBuilt && Blueprint != null)
        {
            if (IsBroken())
                OnBroken();
        }
        else
        {
            if (IsNewlyComplete(out var blueprint))
                OnComplete(blueprint);
        }
    }

    void OnPickedUp()
    {
        if (Blueprint?.KeyBlock != null)
            Blueprint.KeyBlock.OnBroken();
    }

    void OnPlaced()
    {
        CheckForCompletionNextTick();
        NotifyNeighbors();
    }

    void NotifyNeighbors()
    {
        var inRange = Physics.OverlapBox(
            transform.position,
            new Vector3(UpdateRange, UpdateRange, UpdateRange));

        foreach (var col in inRange)
        {
            var buildingBlock = col.gameObject.GetComponent<BuildingBlock>();
            if (buildingBlock != null && buildingBlock.IsEligibleToBuild())
                buildingBlock.CheckForCompletionNextTick();
        }
    }

    void OnBroken()
    {
        foreach (var block in Blueprint.OtherBlocks)
            Break(block);

        Break(this);
        ReadyToBuildText.enabled = false;
        IsKeyGraphics.SetActive(false);

        CanBeBuiltGraphics.SetActive(false);
        BuildInteractable.IsAvailable = false;

        Destroy(_hologram);

        void Break(BuildingBlock block)
        {
            block.CanBeBuilt = false;
            block.CanBeBuiltGraphics.SetActive(false);
            block.Blueprint = null;
        }
    }

    void OnComplete(BlueprintInstance instance)
    {
        CanBeBuilt = true;
        Blueprint = instance;

        BuildInteractable.IsAvailable = true;

        CanBeBuiltGraphics.SetActive(true);
        IsKeyGraphics.SetActive(true);

        ReadyToBuildText.enabled = true;
        ReadyToBuildText.text =
            @$"Ready To Build

[{Blueprint.Building.name}]";

        foreach (var block in instance.OtherBlocks)
        {
            block.CanBeBuilt = true;
            block.Blueprint = instance;
            block.CanBeBuiltGraphics.SetActive(true);
        }

        _hologram = Instantiate(Blueprint.Building.BuildingHologramPrototype);
        _hologram.transform.position = transform.position + Blueprint.Building.BuildingHologramOffsetFromKeyBlock;
    }

    bool IsBroken()
    {
        // if we are already party of a footprint, just let the master block handle it
        if (Blueprint.KeyBlock != this)
            return false;

        // broken if we have no footprint
        return !BlueprintFits(Scan(), Blueprint.Building.Blueprint, out _);
    }

    bool IsNewlyComplete(out BlueprintInstance blueprint)
    {
        blueprint = CheckForMatchingBlueprint();
        return blueprint != null;
    }

    BlueprintInstance CheckForMatchingBlueprint()
    {
        var scan = Scan();
        foreach (var building in Construction.Instance.AvailableBuildings)
            if (BlueprintFits(scan, building.Blueprint, out var otherBlocks))
                return new BlueprintInstance
                {
                    Building = building,
                    KeyBlock = this,
                    OtherBlocks = otherBlocks
                };
        
        return null;
    }

    BuildingBlock[,,] Scan()
    {
        var array = new BuildingBlock[ScanRange * 2 + 1, ScanHeight * 2 + 1, ScanRange * 2 + 1];

        var objects = Physics.OverlapBox(
            transform.position,
            new Vector3(
                ScanRange * Game.UnitDistance,
                ScanHeight * Game.UnitDistance,
                ScanRange * Game.UnitDistance));

        foreach (var obj in objects)
        {
            var buildingBlock = obj.transform.GetComponentAnywhere<BuildingBlock>();
            if (buildingBlock == null || !buildingBlock.IsEligibleToBuild())
                continue; // ignore anything else, we'll deal with validation later

            var offset = buildingBlock.transform.position - transform.position;
            var offsetInt = new Vector3Int(
                (int)(offset.x / Game.UnitDistance),
                (int)(offset.y / Game.UnitDistance),
                (int)(offset.z / Game.UnitDistance));

            var address = new Vector3Int(
                offsetInt.x + ScanRange,
                offsetInt.y + ScanHeight,
                offsetInt.z + ScanRange);

            if (address.x < 0 ||
                address.y < 0 ||
                address.z < 0 ||
                address.x >= array.GetLength(0) ||
                address.y >= array.GetLength(1) ||
                address.z >= array.GetLength(2))
            {
                Debug.LogWarning($"Index out of bounds: {address}");
                continue;
            }

            array[address.x, address.y, address.z] = buildingBlock;
        }

        return array;
    }

    bool BlueprintFits(BuildingBlock[,,] scan, Blueprint blueprint, out List<BuildingBlock> otherBlocks)
    {
        var sampler = new GridSampler();
        sampler.Grid = scan;

        var center = new Vector3Int(ScanRange, ScanHeight, ScanRange); // key block location
        var offset = center - blueprint.KeyBlockLocation;
        sampler.ZeroZeroZeroPosition = offset;

        otherBlocks = new List<BuildingBlock>();

        for (int x = 0; x < blueprint.CellArray.GetLength(0); x++)
        for (int y = 0; y < blueprint.CellArray.GetLength(1); y++)
        for (int z = 0; z < blueprint.CellArray.GetLength(2); z++)
        {
            var sample = sampler.Sample(x, y, z);
            var reference = blueprint.CellArray[x, y, z];

            if (sample == null)
            {
                if (!reference.IsEmpty)
                    return false;
            }
            else
            {
                if (reference.IsEmpty || reference.BuildingBlockColorRequired != sample.Color)
                    return false;

                if (sample != this)
                    otherBlocks.Add(sample);
            }
        }

        return true;
    }

    bool IsEligibleToBuild()
    {
        return PickUp.IsPlaced && Blueprint == null;
    }
}