using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AutoLoader : MonoBehaviour
{
    public PlayerHands Hands;

    public float Range = 10;

    public bool TryPallets = true;
    public bool TryPickups = true;

    void Start()
    {
        Hands.Placed += (s, e) => Reload(e.PickUp.Type);
    }

    void Reload(PickUpType typeId)
    {
        if (!enabled)
            return;

        var pickup = GetPickUp(typeId);
        if (pickup != null)
            Hands.Grab(pickup);
    }

    PickUp GetPickUp(PickUpType typeId)
    {
        if (!TryPallets && !TryPickups)
            return null;

        var inRange = Physics.OverlapBox(transform.position, new Vector3(Range, Range, Range));

        PickUp pickUp = null;

        if (TryPallets)
        {
            var pallet = GetPalletInRange(typeId, inRange);
            if (pallet != null)
                pickUp = pallet.Take(typeId);
        }

        if (pickUp == null && TryPickups)
            pickUp = GetPickUpInRange(typeId, inRange);

        return pickUp;
    }

    Pallet GetPalletInRange(PickUpType typeId, Collider[] inRange)
    {
        foreach (var col in inRange)
        {
            var pallet = col.transform.gameObject.GetComponent<Pallet>();
            if (pallet != null && pallet.CanTake(typeId))
                return pallet;
        }

        return null;
    }

    PickUp GetPickUpInRange(PickUpType typeId, Collider[] inRange)
    {
        foreach (var col in inRange)
        {
            var pickup = col.GetComponent<PickUp>();
            if (pickup == null)
                pickup = col.GetComponentInParent<PickUp>();

            if (pickup != null &&
                pickup.Type == typeId && 
                !pickup.IsPlaced && 
                !pickup.IsPlacing &&
                !pickup.IsLocked)
                return pickup;
        }

        return null;
    }
}