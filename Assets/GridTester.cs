using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
class GridTester : MonoBehaviour
{
    public CashewGrid Grid;

    public float CircleRadius = .25f;
    public int CircleSegments = 6;

    public int DrawDistance;
    public GameObject Pointer;

    void Update()
    {
        for (int x = 1; x <= DrawDistance; x++)
        for (int y = 1; y <= DrawDistance; y++)
        for (int z = 1; z <= DrawDistance; z++)
        {
            DrawCell(Grid.GetCenterOfCell(x, y, z), Grid.HalfUnitDistance, Color.blue);
            DrawCell(Grid.GetCenterOfCell(-x, y, z), Grid.HalfUnitDistance, Color.red);
            DrawCell(Grid.GetCenterOfCell(x, y, -z), Grid.HalfUnitDistance, Color.yellow);
            DrawCell(Grid.GetCenterOfCell(-x, y, -z), Grid.HalfUnitDistance, Color.green);

            DrawCell(Grid.GetCenterOfCell(x, y, z), Grid.HalfUnitDistance * .05f, Color.black);
            DrawCell(Grid.GetCenterOfCell(-x, y, z), Grid.HalfUnitDistance * .15f, Color.white);
            DrawCell(Grid.GetCenterOfCell(x, y, -z), Grid.HalfUnitDistance * .25f, Color.black);
            DrawCell(Grid.GetCenterOfCell(-x, y, -z), Grid.HalfUnitDistance * .35f, Color.white);
        }


        var activeCell = Grid.GetCell(Pointer.transform.position);
        var activeCellPosition = Grid.GetBottomOfCell(activeCell);
        var activeCellCenter = Grid.GetCenterOfCell(activeCell);

        DrawHelper.DrawCircle(activeCellCenter, CircleRadius, CircleSegments, Color.white);
        DrawHelper.DrawCircle(activeCellCenter, CircleRadius, CircleSegments, Color.white, 1);
        DrawHelper.DrawCircle(activeCellCenter, CircleRadius, CircleSegments, Color.white, 2);
        DrawHelper.DrawBox(activeCellPosition, Vector3.one * .25f, Color.magenta);
    }



    void DrawCell(Vector3 center, float halfSide, Color color)
    {
        DrawHelper.DrawBox(center, new Vector3(halfSide, halfSide, halfSide), color);
    }
}