using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
// todo: for export to cashew
namespace Cashew
{
    [Serializable]
    class CashewGrid
    {
        public static float DefaultUnitDistance = .5f;

        public float UnitDistance = .5f;
        public float HalfUnitDistance => UnitDistance * .5f;
        public Vector3 Center;

        public CashewGrid()
            : this(DefaultUnitDistance) { }

        public CashewGrid(float unitDistance)
            : this(unitDistance, Vector3.zero) { }

        public CashewGrid(float unitDistance, Vector3 center)
        {
            UnitDistance = unitDistance;
            Center = center;
        }

        public Vector3 GetBottomOfCell(int x, int y, int z) =>
            GetBottomOfCell(new Vector3Int(x, y, z));

        public Vector3 GetBottomOfCell(Vector3 position) =>
            GetBottomOfCell(GetCell(position - Center));

        public Vector3 GetBottomOfCell(Vector3Int cell)
        {
            ValidateCellInput(cell);
            cell = TransformCellInput(cell);

            return
                Center +
                new Vector3(
                    cell.x * UnitDistance + HalfUnitDistance,
                    cell.y * UnitDistance,
                    cell.z * UnitDistance + HalfUnitDistance);
        }


        public Vector3 GetCenterOfCell(int x, int y, int z) =>
            GetCenterOfCell(new Vector3Int(x, y, z));

        public Vector3 GetCenterOfCell(Vector3 position) =>
            GetCenterOfCell(GetCell(position - Center));

        public Vector3 GetCenterOfCell(Vector3Int cell)
        {
            ValidateCellInput(cell);
            cell = TransformCellInput(cell);

            return
                Center +
                new Vector3(
                    cell.x * UnitDistance + HalfUnitDistance,
                    cell.y * UnitDistance + HalfUnitDistance,
                    cell.z * UnitDistance + HalfUnitDistance);
        }
        
        public Vector3Int GetCell(Vector3 position)
        {
            position -= Center;

            var cell = new Vector3Int(
                (int)(position.x / UnitDistance),
                (int)(position.y / UnitDistance),
                (int)(position.z / UnitDistance));

            if (position.x >= 0)
                cell.x++;
            if (position.y >= 0)
                cell.y++;
            if (position.z >= 0)
                cell.z++;

            if (position.x < 0)
                cell.x--;
            if (position.y < 0)
                cell.y--;
            if (position.z < 0)
                cell.z--;

            return cell;
        }

        void ValidateCellInput(Vector3Int cell)
        {
            if (cell.x == 0 || cell.y == 0 || cell.z == 0)
                throw new ArgumentException("Grid cell positions start at 1. There is no 0,0. Check your cell location.");
        }

        Vector3Int TransformCellInput(Vector3Int cell)
        {
            if (cell.x > 0)
                cell.x--;
            if (cell.y > 0)
                cell.y--;
            if (cell.z > 0)
                cell.z--;
            return cell;
        }
    }

    static class DrawHelper
    {
        public static void DrawBox(Vector3 position, Vector3 halfExtents, Color? color = null)
        {
            Box.FromCenterWithExtents(position, halfExtents, color).Draw();
        }

        public static void DrawCircle(Vector3 position, float radius, int segments, Color? color = null, int orientation = 0)
        {
            Circle.FromRadius(position, radius, segments, color, orientation).Draw();
        }
    }

    class Circle
    {
        public Color Color = Color.white;
        public Vector3[] Points;

        public void Draw()
        {
            for (int i = 0; i < Points.Length - 1; i++)
                Debug.DrawLine(Points[i], Points[i + 1], Color);

            Debug.DrawLine(Points[Points.Length - 1], Points[0], Color);
        }

        // creates a circle on the xz plane
        public static Circle FromRadius(Vector3 position, float radius, int segments, Color? color = null, int orientation = 0)
        {
            if (segments < 3)
                throw new InvalidOperationException();

            var circle = new Circle();
            circle.Points = new Vector3[segments];
            circle.Color = color ?? Color.white;

            for (int i = 0; i < segments; i++)
            {
                var angle = i * (360 / segments);

                if (orientation == 0)
                    circle.Points[i] =
                        position +
                        new Vector3(
                            Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
                            0,
                            Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
                else if (orientation == 1)
                    circle.Points[i] =
                        position +
                        new Vector3(
                            0,
                            Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
                            Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
                else if (orientation == 2)
                    circle.Points[i] =
                        position +
                        new Vector3(
                            Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
                            Mathf.Sin(Mathf.Deg2Rad * angle) * radius,
                            0);
                else
                    throw new ArgumentOutOfRangeException();
            }
            
            return circle;
        }
    }

    class Box
    {
        public Color Color = Color.white;

        public Vector3 TopNorthWest;
        public Vector3 TopNorthEast;
        public Vector3 TopSouthWest;
        public Vector3 TopSouthEast;

        public Vector3 BottomNorthWest;
        public Vector3 BottomNorthEast;
        public Vector3 BottomSouthWest;
        public Vector3 BottomSouthEast;

        public void Draw()
        {
            // connect top to bottom
            Debug.DrawLine(TopNorthWest, BottomNorthWest, Color);
            Debug.DrawLine(TopNorthEast, BottomNorthEast, Color);
            Debug.DrawLine(TopSouthWest, BottomSouthWest, Color);
            Debug.DrawLine(TopSouthEast, BottomSouthEast, Color);

            // connect top
            Debug.DrawLine(TopNorthWest, TopNorthEast, Color);
            Debug.DrawLine(TopNorthEast, TopSouthEast, Color);
            Debug.DrawLine(TopSouthEast, TopSouthWest, Color);
            Debug.DrawLine(TopSouthWest, TopNorthWest, Color);

            // connect bottom
            Debug.DrawLine(BottomNorthWest, BottomNorthEast, Color);
            Debug.DrawLine(BottomNorthEast, BottomSouthEast, Color);
            Debug.DrawLine(BottomSouthEast, BottomSouthWest, Color);
            Debug.DrawLine(BottomSouthWest, BottomNorthWest, Color);
        }

        public static Box FromCenterWithExtents(Vector3 center, Vector3 halfExtents, Color? color = null)
        {
            var box = new Box();

            box.Color = color ?? Color.white;

            // top

            box.TopNorthWest = new Vector3(
                center.x - halfExtents.x,
                center.y + halfExtents.y,
                center.z + halfExtents.z);

            box.TopNorthEast = new Vector3(
                center.x + halfExtents.x,
                center.y + halfExtents.y,
                center.z + halfExtents.z);

            box.TopSouthEast = new Vector3(
                center.x + halfExtents.x,
                center.y + halfExtents.y,
                center.z - halfExtents.z);

            box.TopSouthWest = new Vector3(
                center.x - halfExtents.x,
                center.y + halfExtents.y,
                center.z - halfExtents.z);

            // bottom

            box.BottomNorthWest = new Vector3(
                center.x - halfExtents.x,
                center.y - halfExtents.y,
                center.z + halfExtents.z);

            box.BottomNorthEast = new Vector3(
                center.x + halfExtents.x,
                center.y - halfExtents.y,
                center.z + halfExtents.z);

            box.BottomSouthEast = new Vector3(
                center.x + halfExtents.x,
                center.y - halfExtents.y,
                center.z - halfExtents.z);

            box.BottomSouthWest = new Vector3(
                center.x - halfExtents.x,
                center.y - halfExtents.y,
                center.z - halfExtents.z);

            return box;
        }
    }
}