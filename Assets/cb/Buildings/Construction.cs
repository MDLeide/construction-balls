using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

class Construction : MonoBehaviour
{
    const string AssetPath = "Assets/cb/Buildings/Blueprints";

    public static Construction Instance;

    [ReadOnly]
    public Building[] Buildings;
    [ReadOnly]
    public List<Building> AvailableBuildings;


    public event EventHandler<NewBuildingEventArgs> NewBuildingAvailable; 


    void Start()
    {
        Instance = this;
        Research.Instance.ItemResearched += ItemResearched;

        Buildings = GetAllBuildings();

        foreach (var b in Buildings)
        {
            if (IsAvailable(b))
                AvailableBuildings.Add(b);

            if (b.ID <= 0 && Game.Instance.WarnOnInvalidID)
                Debug.LogWarning($"Building has invalid ID: {b.name}");
        }
    }

    Building[] GetAllBuildings()
    {
        return AssetDatabaseHelper.LoadAssetsFromFolder<Building>(AssetPath);
    }

    bool IsAvailable(Building building)
    {
        return building.RequiredResearch == null || Research.Instance.IsResearched(building.RequiredResearch);
    }

    void ItemResearched(object sender, ResearchEventArgs e)
    {
        var newBuildings = Buildings.Where(p => p.RequiredResearch == e.Research).ToArray();
        AvailableBuildings.AddRange(newBuildings);

        foreach (var b in newBuildings)
            NewBuildingAvailable?.Invoke(this, new NewBuildingEventArgs(b));
    }
}