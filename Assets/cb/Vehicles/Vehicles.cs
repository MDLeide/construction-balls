using System.Collections.Generic;
using UnityEngine;

class Vehicles : MonoBehaviour
{
    public static Vehicles Instance;

    public List<GameObject> AllVehicles;

    public int MaximumCapacity;
    public int CurrentCapacity;

    public bool CanSupport(int additionalCapacity)
    {
        return additionalCapacity <= MaximumCapacity - CurrentCapacity;
    }

    public void RegisterVehicle(GameObject vehicle)
    {
        AllVehicles.Add(vehicle);
    }
}