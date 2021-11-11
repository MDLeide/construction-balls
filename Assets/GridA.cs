using UnityEngine;

class GridA
{
    public GridA(float unitDistance, Vector3? zeroZero = null)
    {
        UnitDistance = unitDistance;
        HalfUnitDistance = unitDistance / 2;
        ZeroZero = zeroZero ?? Vector3.zero;
    }

    public float UnitDistance { get; set; }
    public float HalfUnitDistance { get; set; }
    public Vector3 ZeroZero { get; set; }

    public Vector3 GetBottomPointOfCell(Vector3 position)
    {
        position -= ZeroZero;

        var cell = GetCell(position);

        return new Vector3(
            cell.x * UnitDistance + HalfUnitDistance,
            cell.y * UnitDistance,
            cell.z * UnitDistance + HalfUnitDistance);
    }

    public Vector3 GetCenterPointOfCell(Vector3 position)
    {
        position -= ZeroZero;

        var cell = GetCell(position);

        return new Vector3(
            cell.x * UnitDistance + HalfUnitDistance,
            cell.y * UnitDistance + HalfUnitDistance,
            cell.z * UnitDistance + HalfUnitDistance);
    }

    public Vector3Int GetCell(Vector3 position)
    {
        position -= ZeroZero;

        var cell = new Vector3Int(
            (int) (position.x / UnitDistance),
            (int) (position.y / UnitDistance),
            (int) (position.z / UnitDistance));

        if (position.x < 0)
            cell.x--;
        if (position.y < 0)
            cell.y--;
        if (position.z < 0)
            cell.z--;

        return cell;
    }
}