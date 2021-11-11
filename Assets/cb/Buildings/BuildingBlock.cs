using System;
using System.Collections.Generic;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

class BuildingBlock : MonoBehaviour
{
    const int CompletionFrameDelay = 2;
    const int ScanRange = 50;
    const int ScanHeight = 25;

    GameObject _hologram;

    [Header("Config")]
    public PickUp PickUp;

    public NetworkBlock NetworkBlock;
    [Space]
    public GameObject CanBeBuiltGraphics;
    public GameObject IsKeyGraphics;
    [Space]
    public TMP_Text ReadyToBuildText;
    public Interactable BuildInteractable;

    [Space]
    public LayerMask BuildingBlockLayerMask;

    [Header("Settings")]
    [Tooltip("How far to search when updating other building blocks in the area after being placed.")]
    public float UpdateRange = 15f;
    public BallColor Color = BallColor.Blue;

    [Header("State")]

    [Tooltip("This is the blueprint that the building block is currently participating in, if applicable. Null otherwise.")]
    [ReadOnly]
    public BlueprintInstance Blueprint;
    [ReadOnly]
    public bool CanBeBuilt;


    void Start()
    {
        Blueprint = null;
        NetworkBlock.NetworkUpdated += NetworkUpdated;
        PickUp.PickedUp += (sender, args) => OnPickedUp();
        BuildInteractable.Pushed += OnBuild;
    }

    void NetworkUpdated(object sender, EventArgs e)
    {
        CheckCompletion();
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

    public void CheckCompletion()
    {
        if (!PickUp.IsPlaced && !PickUp.IsPlacing
            || PickUp.IsOnPallet)
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
    
    // fires when blueprint is broken
    // this is always called by the keyblock
    void OnBroken()
    {
        Destroy(Blueprint.KeyBlock._hologram);

        foreach (var block in Blueprint.OtherBlocks)
            Break(block);

        Break(this);
        ReadyToBuildText.enabled = false;
        IsKeyGraphics.SetActive(false);

        CanBeBuiltGraphics.SetActive(false);
        BuildInteractable.IsAvailable = false;

        void Break(BuildingBlock block)
        {
            block.CanBeBuilt = false;
            block.CanBeBuiltGraphics.SetActive(false);
            block.Blueprint = null;
        }
    }

    // fires when a blueprint has been completed
    void OnComplete(BlueprintInstance instance)
    {
        CanBeBuilt = true;
        Blueprint = instance;

        BuildInteractable.IsAvailable = true;

        instance.KeyBlock.Blueprint = instance;

        instance.KeyBlock.CanBeBuiltGraphics.SetActive(true);
        instance.KeyBlock.IsKeyGraphics.SetActive(true);

        instance.KeyBlock.ReadyToBuildText.enabled = true;
        instance.KeyBlock.ReadyToBuildText.text =
            @$"Ready To Build

[{Blueprint.Building.name}]";

        foreach (var block in instance.OtherBlocks)
        {
            block.CanBeBuilt = true;
            block.Blueprint = instance;
            block.CanBeBuiltGraphics.SetActive(true);
        }
        instance.KeyBlock._hologram = Instantiate(Blueprint.Building.BuildingHologramPrototype);
        instance.KeyBlock._hologram.transform.position =
            instance.Building.Blueprint.GetHologramPosition(instance);
        //instance.KeyBlock._hologram.transform.position = instance.KeyBlock.NetworkBlock.Block.TargetPosition;
        //+ Blueprint.Building.BuildingHologramOffsetFromKeyBlock;
    }

    // true if we just broke a blueprint
    bool IsBroken()
    {
        // if we are already party of a footprint, just let the master block handle it
        if (Blueprint.KeyBlock != this)
            return false;

        // broken if we have no footprint
        return !Blueprint.Building.Blueprint.MatchesBlockArray(NetworkBlock.BlockNetwork.Network, out _, out _);
    }

    // true if we just completed a blueprint
    bool IsNewlyComplete(out BlueprintInstance blueprint)
    {
        blueprint = CheckForMatchingBlueprint();
        return blueprint != null;
    }

    // searches the blueprint collection to determine if the current network matches any
    BlueprintInstance CheckForMatchingBlueprint()
    {
        foreach (var building in Construction.Instance.AvailableBuildings)
            if (building.Blueprint.MatchesBlockArray(NetworkBlock.BlockNetwork.Network, out var rotations, out var keyBlock))
            {
                var bp = new BlueprintInstance
                {
                    Building = building,
                    KeyBlock = keyBlock,
                    //KeyBlock = building.Blueprint.GetKeyBlock(NetworkBlock.BlockNetwork.Network, rotations),
                    OtherBlocks = new List<BuildingBlock>()
                };

                for (int x = 0; x < NetworkBlock.BlockNetwork.Network.GetLength(0); x++)
                for (int y = 0; y < NetworkBlock.BlockNetwork.Network.GetLength(1); y++)
                for (int z = 0; z < NetworkBlock.BlockNetwork.Network.GetLength(2); z++)
                {
                    var netBlock = NetworkBlock.BlockNetwork.Network[x, y, z];
                    if (netBlock == null)
                        continue;

                    var buildingBlock = netBlock.GetComponentAnywhere<BuildingBlock>();
                    if (buildingBlock == null)
                        continue;

                    if (buildingBlock != bp.KeyBlock)
                        bp.OtherBlocks.Add(buildingBlock);
                }

                return bp;
            }

        return null;
    }
}