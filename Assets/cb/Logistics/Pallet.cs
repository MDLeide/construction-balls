using System;
using System.Linq;
using UnityEngine;

[SelectionBase]
class Pallet : MonoBehaviour
{
    public PlacementLocation[] PlacementLocations;
    public Vector3 LocationRotation;

    public string PlaceMessage = "Press E To Place";
    public string TakeMessage = "Press E To Take";

    public PickUpType[] AcceptablePickUpIds;

    public bool CanPlace(PickUp pickup)
    {
        return AcceptablePickUpIds.Contains(pickup.Type) && PlacementLocations.Any(p => p.Occupant == null);
    }

    public bool CanTake()
    {
        return PlacementLocations.Any(p => p.Occupant == null);
    }

    public bool CanTake(PickUpType pickUpTypeId)
    {
        return PlacementLocations.Any(p => p.Occupant != null && p.Occupant.Type == pickUpTypeId);
    }

    public bool Place(PickUp pickup)
    {
        PlacementLocation location = null;

        for (int i = 0; i < PlacementLocations.Length; i++)
            if (PlacementLocations[i].Occupant == null)
            {
                location = PlacementLocations[i];
                break;
            }
        
        if (location == null)
            return false;

        location.Occupant = pickup;
        
        pickup.IsOnPallet = true;
        pickup.transform.parent = location.transform;
        pickup.PickedUp += OnPickedUp;
        pickup.Place(Vector3.zero, Quaternion.identity, true);
        return true;
    }

    public PickUp Take()
    {
        PlacementLocation location = null;
        for (int i = PlacementLocations.Length - 1; i >= 0; i--)
        {
            if (PlacementLocations[i].Occupant != null)
            {
                location = PlacementLocations[i];
                break;
            }
        }

        if (location == null)
            return null;
        
        var pickup = location.Occupant;
        pickup.PickedUp += OnPickedUp;
        location.Occupant = null;
        return pickup;
    }

    public PickUp Take(PickUpType pickUpType)
    {
        PlacementLocation location = null;
        for (int i = PlacementLocations.Length - 1; i >= 0; i--)
        {
            if (PlacementLocations[i].Occupant != null && PlacementLocations[i].Occupant.Type == pickUpType)
            {
                location = PlacementLocations[i];
                break;
            }
        }

        if (location == null)
            return null;
        
        var pickup = location.Occupant;
        location.Occupant = null;
        pickup.PickedUp -= OnPickedUp;
        return pickup;
    }

    void OnPickedUp(object sender, EventArgs args)
    {
        var pickup = sender as PickUp;
        if (pickup == null)
            return;

        var loc = PlacementLocations.FirstOrDefault(p => p.Occupant == pickup);
        if (loc != null)
            loc.Occupant = null;

        pickup.PickedUp -= OnPickedUp;
    }
}