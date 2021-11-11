using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Build.Content;
using UnityEngine;


static class GridHelper
{
    public static Vector3 GetBottomPointOfCell(Vector3 position)
    {
        var cell = GetCell(position);

        return new Vector3(
            cell.x * Game.UnitDistance + Game.HalfUnitDistance,
            cell.y * Game.UnitDistance,
            cell.z * Game.UnitDistance + Game.HalfUnitDistance);
    }

    public static Vector3 GetCenterPointOfCell(Vector3 position)
    {
        var cell = GetCell(position);

        return new Vector3(
            cell.x * Game.UnitDistance + Game.HalfUnitDistance,
            cell.y * Game.UnitDistance + Game.HalfUnitDistance,
            cell.z * Game.UnitDistance + Game.HalfUnitDistance);
    }

    public static Vector3Int GetCell(Vector3 position)
    {
        var cell = new Vector3Int(
            (int)(position.x / Game.UnitDistance),
            (int)(position.y / Game.UnitDistance),
            (int)(position.z / Game.UnitDistance));

        if (position.x < 0)
            cell.x--;
        if (position.y < 0)
            cell.y--;
        if (position.z < 0)
            cell.z--;

        return cell;
    }
}