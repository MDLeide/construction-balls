using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

class PalletLayout : MonoBehaviour
{
    public Pallet Pallet;

    public int XCount = 5;
    public int ZCount = 5;
    public int YCount = 1;

    public float XSpace = .5f;
    public float ZSpace = .5f;
    public float YSpace = .5f;

    public Vector3 Offset = new Vector3(0, .3f, 0);

    public PlacementLocation PlacementLocation;

    [Button]
    public void Layout()
    {
        var mountPoints = new GameObject("mount points");
        mountPoints.transform.parent = Pallet.transform;
        mountPoints.transform.localPosition = Offset;

        var offset = new Vector3(
            (XCount - 1) * XSpace / -2,
            0,
            (ZCount - 1) * ZSpace / -2);

        var points = new List<PlacementLocation>();


        for (int y = 0; y < YCount; y++) 
        for (int x = 0; x < XCount; x++)
        for (int z = 0; z < ZCount; z++)
        {
            var p = (PlacementLocation) PrefabUtility.InstantiatePrefab(
                PlacementLocation,
                mountPoints.transform);

            p.gameObject.name = $"{x},{y},{z}";
            p.transform.parent = mountPoints.transform;
            p.transform.localPosition = new Vector3(x * XSpace, y * YSpace, z * ZSpace) + offset;
            points.Add(p);
        }

        Pallet.PlacementLocations = points.ToArray();
    }
}