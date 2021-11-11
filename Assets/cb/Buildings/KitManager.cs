using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

class KitManager : MonoBehaviour
{
    [Button]
    public void CreateKit(string kitName)
    {
        var kitObject = new GameObject();
        kitObject.name = kitName;
        kitObject.transform.parent = transform;

        var kit = kitObject.AddComponent<BuildingKit>();
        kit.Setup();
    }

    [Button]
    public void SaveAll()
    {
        var children = gameObject.GetChildren();
        var count = 0;
        foreach (var child in children)
        {
            count++;
            var kit = child.GetComponentAnywhere<BuildingKit>();
            if (kit != null)
            {
                var orig = kit.gameObject.activeSelf;
                kit.gameObject.SetActive(true);
                kit.Save();
                kit.gameObject.SetActive(orig);
            }
        }

        Debug.Log($"Save all complete. Saved {count} buildings.");
    }
}