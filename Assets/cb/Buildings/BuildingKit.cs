using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

class BuildingKit : MonoBehaviour
{
    public string PathName = "Assets/cb/Buildings/Blueprints";

    const string BuildingHologramName = "Building Hologram";
    const string LayoutHologramName = "Layout Hologram";
    const string BuildingPrototypeName = "Building Prototype";
    const string BlueprintName = "Blueprint";
    const string BuildingName = "Building";

    public GameObject BuildingHologram;
    public GameObject LayoutHologram;
    public GameObject BuildingPrototype;
    public KeyBuildingBlock KeyBlock;

    void Unpack()
    {
        try
        {
            PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
        }
        catch
        {
            //
        }
    }

    [Button]
    public void Save()
    {
        Unpack();

        if (BuildingHologram == null ||
            LayoutHologram == null ||
            BuildingPrototype == null ||
            KeyBlock == null)
        {
            Debug.LogWarning("Failure on save, something is null");
            return;
        }

        var blueprint = KeyBlock.MakeBlueprint();
     
        SaveAsset(blueprint, BlueprintName);
        
        SavePrefab(BuildingHologram, BuildingHologramName);
        SavePrefab(LayoutHologram, LayoutHologramName);
        SavePrefab(BuildingPrototype, BuildingPrototypeName);

        var building = (Building) ScriptableObject.CreateInstance(typeof(Building));
        building.BuildingHologramPrototype =
            AssetDatabase.LoadAssetAtPath<GameObject>(GetFullPath(BuildingHologramName));

        building.LayoutHologramPrototype =
            AssetDatabase.LoadAssetAtPath<GameObject>(GetFullPath(LayoutHologramName));

        building.BuildingPrototype =
            AssetDatabase.LoadAssetAtPath<GameObject>(GetFullPath(BuildingPrototypeName));

        building.Blueprint = blueprint;
        building.BuildingOffsetFromKeyBlock = BuildingPrototype.transform.position - KeyBlock.transform.position;
        building.BuildingHologramOffsetFromKeyBlock = BuildingHologram.transform.position - KeyBlock.transform.position;

        SaveAsset(building, BuildingName);

        UpdateConstruction();
    }

    void UpdateConstruction()
    {
        const string GamePath = "Assets/cb/Game.prefab";

        var game = AssetDatabase.LoadAssetAtPath<GameObject>(GamePath);
        var construction = game.GetComponentAnywhere<Construction>();
        construction.Buildings = GetAllBuildings().ToArray();
        PrefabUtility.SavePrefabAsset(game);
    }

    List<Building> GetAllBuildings()
    {
        var subFolders = AssetDatabase.GetSubFolders(PathName);
        var buildings = new List<Building>();

        foreach (var sub in subFolders)
        {
            var assets = AssetDatabase.FindAssets(" - Building", new[] {sub});
            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<Building>(path);
                if (asset == null)
                    continue;

                Debug.Log(path);
                buildings.Add(asset);
            }
        }

        return buildings;
    }

    void SavePrefab(GameObject obj, string typeName)
    {
        if (!AssetDatabase.IsValidFolder(GetFolderPath()))
            AssetDatabase.CreateFolder(PathName, GetBaseName());

        PrefabUtility.SaveAsPrefabAsset(obj, GetFullPath(typeName, "prefab"));
    }

    void SaveAsset(Object obj, string typeName)
    {
        if (!AssetDatabase.IsValidFolder(GetFolderPath()))
            AssetDatabase.CreateFolder(PathName, GetBaseName());

        AssetDatabase.CreateAsset(obj, GetFullPath(typeName, "asset"));
        AssetDatabase.SaveAssets();
    }

    string GetFolderPath()
    {
        return PathName + "/" + GetBaseName();
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