using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class VehicleStation : MonoBehaviour
{
    public int Capacity;

    public CraftingStation CraftingStation;

    void Start()
    {
        Vehicles.Instance.MaximumCapacity += Capacity;
        CraftingStation.CanCraft += CanCraft;
        CraftingStation.StartedCrafting += StartedCrafting;
    }

    void StartedCrafting(object sender, CraftingStartedEventArgs e)
    {
        var vr = CraftingStation.SelectedRecipe as VehicleRecipe;
        if (vr == null)
            return;

        Vehicles.Instance.CurrentCapacity += vr.CapacityCost;
    }

    bool CanCraft()
    {
        var vr = CraftingStation.SelectedRecipe as VehicleRecipe;
        if (vr == null)
            return false;

        return Vehicles.Instance.CanSupport(vr.CapacityCost);
    }
}