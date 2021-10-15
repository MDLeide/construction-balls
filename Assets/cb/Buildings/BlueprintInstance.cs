using System;
using System.Collections.Generic;

[Serializable]
class BlueprintInstance
{
    public Building Building;
    public BuildingBlock KeyBlock;
    public List<BuildingBlock> OtherBlocks;
}