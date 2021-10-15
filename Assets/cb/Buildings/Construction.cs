using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Construction : MonoBehaviour
{
    public static Construction Instance;

    public Building[] Buildings;
    public List<Building> AvailableBuildings;

    public event EventHandler<NewBuildingEventArgs> NewBuildingAvailable; 


    void Start()
    {
        Instance = this;
        Research.Instance.ItemResearched += ItemResearched;

        foreach (var b in Buildings)
            if (IsAvailable(b))
                AvailableBuildings.Add(b);
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

class NewBuildingEventArgs : EventArgs
{
    public NewBuildingEventArgs(Building building)
    {
        NewBuilding = building;
    }

    public Building NewBuilding { get; }
}