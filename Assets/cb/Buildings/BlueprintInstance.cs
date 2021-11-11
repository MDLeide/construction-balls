using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class BlueprintInstance
{
    public Building Building;
    public BuildingBlock KeyBlock;
    public List<BuildingBlock> OtherBlocks;

    public Vector3 HologramOffset;
    public int Rotations;
}