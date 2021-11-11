using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


[SelectionBase]
class BuildingBlockWrapper : MonoBehaviour
{
    GameObject _buildingBlockWrapperPrefab;

    GameObject BuildingBlockWrapperPrefab
    {
        get
        {
            if (_buildingBlockWrapperPrefab == null)
                _buildingBlockWrapperPrefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>("Assets/cb/Buildings/Blueprints/block wrapper.prefab");

            return _buildingBlockWrapperPrefab;
        }
    }

    GameObject CreateBuildingBlock()
    {
        return PrefabUtility.InstantiatePrefab(BuildingBlockWrapperPrefab) as GameObject;
    }

    [Button]
    public void ConnectLines()
    {
        Connect(Vector3.forward);
        Connect(Vector3.right);
        Connect(Vector3.left);
        Connect(Vector3.back);
        Connect(Vector3.down);
        Connect(Vector3.up);
    }

    [Button]
    public void ConnectLine(CompassDirection direction)
    {
        switch (direction)
        {
            case CompassDirection.North:
                Connect(Vector3.forward);
                break;
            case CompassDirection.East:
                Connect(Vector3.right);
                break;
            case CompassDirection.South:
                Connect(Vector3.back);
                break;
            case CompassDirection.West:
                Connect(Vector3.left);
                break;
            case CompassDirection.Up:
                Connect(Vector3.up);
                break;
            case CompassDirection.Down:
                Connect(Vector3.down);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    void Connect(Vector3 direction)
    {
        var pos = transform.position + Vector3.up * Game.UnitDistance / 2;
        pos += new Vector3(.25f, 0, .25f);
        var ray = new Ray(pos, direction * 5);
        if (Physics.Raycast(ray, out var hit))
        {
            var otherBuildingBlock = hit.transform.GetComponentAnywhere<BuildingBlockWrapper>();
            if (otherBuildingBlock == null)
                return;

            var distance = (otherBuildingBlock.transform.position - transform.position).magnitude - Game.UnitDistance;

            var blocks = (int)(distance / Game.UnitDistance);
            for (int i = 0; i < blocks; i++)
            {
                var b = CreateBuildingBlock();
                Undo.RegisterCreatedObjectUndo(b, "Create Building Block Wrapper");
                b.transform.parent = transform.parent;
                b.transform.position = transform.position + direction * (i + 1) * Game.UnitDistance;
            }
        }
    }
}