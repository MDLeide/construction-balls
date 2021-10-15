using System.Collections;
using System.Collections.Generic;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

class FloorLayout : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Offset;
    
    [Header("Floor")]
    public Transform Panel;
    public Vector3 PanelOffset = new Vector3(2.75f, 0, 2.75f);
    [Header("Panel Dimensions in Units")]
    public float PanelWidth = 5;
    public float PanelHeight = 5;
    [Header("Floor Dimensions in Panel Units")]
    public float Width = 5;
    public float Height = 5;

    [Button]
    public void Layout()
    {
        foreach (var child in gameObject.GetAllChildren())
            DestroyImmediate(child);

        var offset = new Vector3(
                Width * PanelWidth / -2,
                0,
                Height * PanelHeight / -2);

        MakeFloor(offset);
    }

    void MakeFloor(Vector3 offset)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var panel = (Transform)PrefabUtility.InstantiatePrefab(Panel, transform);
                panel.localPosition = new Vector3(x * PanelWidth, 0, y * PanelHeight) + offset + PanelOffset;
            }
        }
    }
}
