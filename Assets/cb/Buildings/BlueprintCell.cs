using System;
using UnityEngine;

[Serializable]
class BlueprintCell
{
    public Vector3Int Location;
    public bool IsEmpty;
    public BallColor BuildingBlockColorRequired;

    public static BlueprintCell Empty(Vector3Int location)
    {
        return new BlueprintCell()
        {
            IsEmpty = true,
            Location = location
        };
    }

    public static BlueprintCell Occupied(Vector3Int location, BallColor color)
    {
        return new BlueprintCell()
        {
            Location = location,
            BuildingBlockColorRequired = color
        };
    }

    public override string ToString()
    {
        if (IsEmpty)
            return $"{Location} = Empty";

        return $"{Location} = {BuildingBlockColorRequired}";
    }
}