using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


class BuildingKit : MonoBehaviour
{
    const string BlueprintAssetPath = "Assets/cb/Buildings/Blueprints";
    const string ResearchAssetPath = "Assets/cb/Research/Research Items/Buildings";
    const string BuildingHologramName = "Building Hologram";
    const string LayoutHologramName = "Layout Hologram";
    const string BuildingPrototypeName = "Building Prototype";
    const string BlueprintName = "Blueprint";
    const string BuildingName = "Building";
    const string KitName = "Kit";

    [Space]
    public GameObject BuildingPrototype;
    [Space]
    public GameObject BuildingHologram;
    public GameObject LayoutHologram;
    [Space]
    public KeyBuildingBlock KeyBlock;

    [Space]
    public float HologramScale = .1f;

    void Unpack()
    {
        // we can't save a prefab instance as an asset (right?) so we unpack ourselves here
        try
        {
            PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
        }
        catch
        {
            // catch and eat the exception if we aren't a prefab instance
        }
    }

    [Button]
    [PropertySpace]
    [PropertyOrder(-10)]
    public void Setup()
    {
        var buildingName = GetBaseName();
        var layout = new GameObject($"{buildingName} Layout");
        layout.transform.parent = transform;
        LayoutHologram = layout;

        var building = new GameObject(buildingName);
        building.transform.parent = transform;
        BuildingPrototype = building;

        var buildingHologram = new GameObject($"{buildingName} Hologram");
        buildingHologram.transform.parent = transform;
        var hologramComponent = buildingHologram.AddComponent<Hologram>();
        hologramComponent.CraftHologramScale = Vector3.one * HologramScale;
        
        BuildingHologram = buildingHologram;

        var keyBlockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/cb/Buildings/Blueprints/key block wrapper.prefab");
        var keyBlock = PrefabUtility.InstantiatePrefab(keyBlockPrefab, layout.transform) as GameObject;

        KeyBlock = keyBlock.GetComponentAnywhere<KeyBuildingBlock>();

        var blockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/cb/Buildings/Blueprints/block wrapper.prefab");
        var block = PrefabUtility.InstantiatePrefab(blockPrefab, layout.transform) as GameObject;
        block.transform.localPosition = Vector3.right * 2;
    }

    [Button]
    [PropertySpace]
    [PropertyOrder(1)]
    public void Save()
    {
        // check to ensure we are in a good state to save
        Guard();

        // unpack any existing prefabs at the top level, they will cause issues
        // when we are saving
        Unpack();

        // create and save the blueprint (blueprint is data controlling the building block layout)
        var blueprint = KeyBlock.MakeBlueprint();
        SaveAsset(blueprint, BlueprintName);
        
        // save holograms
        SavePrefab(BuildingHologram, BuildingHologramName);
        SavePrefab(LayoutHologram, LayoutHologramName);

        // save the actual building prefab, this is used when the building blocks are placed
        // and build is executed
        SavePrefab(BuildingPrototype, BuildingPrototypeName);

        // create the research item for this building
        var research = GetResearchItem(); // creates or gets an existing research item
        research.DisplayHologram = AssetDatabase.LoadAssetAtPath<Hologram>(GetFullPath(BuildingHologramName));

        // create the building scriptableobject, which is basically a container for everything
        // we've created so far
        var building = MakeBuilding(blueprint);
        building.RequiredResearch = research;
        
        SaveAsset(building, BuildingName);

        SavePrefab(gameObject, KitName);

        AssetDatabase.Refresh();
        Debug.Log($"Save Complete: {name}");
    }

    [Button]
    [PropertySpace]
    [PropertyOrder(-1)]
    public void UpdateData()
    {
        if (LayoutHologram != null)
            LayoutHologram.name = $"{GetBaseName()} Layout";

        if (BuildingPrototype != null)
            BuildingPrototype.name = GetBaseName();

        if (BuildingHologram != null)
            BuildingHologram.name = $"{GetBaseName()} Hologram";

        var hologram = BuildingHologram.GetComponent<Hologram>();
        if (hologram == null)
        {
            hologram = BuildingHologram.AddComponent<Hologram>();
            hologram.CraftHologramScale = Vector3.one * HologramScale;
        }
    }

    void Guard()
    {
        if (BuildingHologram == null ||
            LayoutHologram == null ||
            BuildingPrototype == null ||
            KeyBlock == null)
            throw new InvalidOperationException("Failure on save, something is null");
    }

    ResearchItem GetResearchItem()
    {
        var path = ResearchAssetPath + $"/{GetBaseName()}.asset";

        var existing = AssetDatabase.LoadAssetAtPath<ResearchItem>(path);
        if (existing != null)
            return existing;

        var researchItem = ScriptableObject.CreateInstance<ResearchItem>();
        researchItem.name = GetBaseName();
        researchItem.ID = -1;
        AssetDatabase.CreateAsset(researchItem, path);
        return researchItem;
    }
    
    Building MakeBuilding(Blueprint blueprint)
    {
        var building = (Building)ScriptableObject.CreateInstance(typeof(Building));
        building.BuildingHologramPrototype =
            AssetDatabase.LoadAssetAtPath<GameObject>(GetFullPath(BuildingHologramName));

        building.LayoutHologramPrototype =
            AssetDatabase.LoadAssetAtPath<GameObject>(GetFullPath(LayoutHologramName));

        building.BuildingPrototype =
            AssetDatabase.LoadAssetAtPath<GameObject>(GetFullPath(BuildingPrototypeName));

        building.Blueprint = blueprint;
        building.BuildingOffsetFromKeyBlock = BuildingPrototype.transform.position - KeyBlock.transform.position;
        building.BuildingHologramOffsetFromKeyBlock = BuildingHologram.transform.position - KeyBlock.transform.position;

        return building;
    }

    void SavePrefab(GameObject obj, string typeName)
    {
        if (!AssetDatabase.IsValidFolder(GetFolderPath()))
            AssetDatabase.CreateFolder(BlueprintAssetPath, GetBaseName());

        PrefabUtility.SaveAsPrefabAsset(obj, GetFullPath(typeName, "prefab"));
    }

    void SaveAsset(Object obj, string typeName)
    {
        if (!AssetDatabase.IsValidFolder(GetFolderPath()))
            AssetDatabase.CreateFolder(BlueprintAssetPath, GetBaseName());

        AssetDatabase.CreateAsset(obj, GetFullPath(typeName, "asset"));
    }

    string GetFolderPath()
    {
        return BlueprintAssetPath + "/" + GetBaseName();
    }

    string GetBaseName()
    {
        return name.Replace("Kit", "").Replace("kit", "").Trim();
    }

    string GetFullPath(string typeName, string suffix = "prefab")
    {
        return $"{GetFolderPath()}/{GetFileName(typeName, suffix)}";
    }

    string GetFileName(string typeName, string suffix)
    {
        return $"{GetBaseName()} - {typeName}.{suffix}";
    }
}