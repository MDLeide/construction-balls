using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

class PanelLayout : MonoBehaviour
{
    public float Width;
    public float Length;

    public int XCount;
    public int ZCount;

    public Transform Parent;
    public Panel Panel;

    [Button]
    public void Layout()
    {
        var spacing = new Vector3(
            Width / XCount,
            0,
            Length / ZCount);

        var offset = new Vector3(
            spacing.x / 2,
            0,
            spacing.z / 2);

        for (int x = 0; x < XCount; x++)
        {
            for (int z = 0; z < ZCount; z++)
            {
                var p = (Panel)PrefabUtility.InstantiatePrefab(Panel, Parent);
                
                p.transform.localPosition =
                    offset +
                    new Vector3(
                        spacing.x * x,
                        0,
                        spacing.z * z);
            }
        }
    }
}