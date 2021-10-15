using UnityEngine;

class GridSampler
{
    public BuildingBlock[,,] Grid;
    public Vector3Int ZeroZeroZeroPosition;

    public BuildingBlock Sample(int x, int y, int z)
    {
        return Grid[ZeroZeroZeroPosition.x + x,
            ZeroZeroZeroPosition.y + y,
            ZeroZeroZeroPosition.z + z];
    }

    public BuildingBlock Sample(Vector3Int position)
    {
        return Grid[position.x + ZeroZeroZeroPosition.x,
            position.y + ZeroZeroZeroPosition.y,
            position.z + ZeroZeroZeroPosition.z];
    }
}