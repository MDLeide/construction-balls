using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using UnityEngine;


[SelectionBase]
class ItemSpawner : MonoBehaviour
{
    int _count;

    public GameObject ObjectToSpawn;
    public bool TryToPlaceOnPallet = true;
    public float PalletRange = 10;
    public LayerMask PalletLayer;

    public void Spawn()
    {
        var obj = Instantiate(ObjectToSpawn, transform.position, transform.rotation);
        obj.name += $" [{_count++}]";
        if (!TryToPlaceOnPallet)
            return;

        var pickup = obj.GetComponentAnywhere<PickUp>();
        if (pickup == null)
            return;
     
        var pallets = GetPalletsInRange();
        foreach (var pallet in pallets)
            if (pallet.Place(pickup))
                return;
    }

    IEnumerable<Pallet> GetPalletsInRange()
    {
        var colliders = Physics.OverlapBox(transform.position, new Vector3(PalletRange, PalletRange, PalletRange), Quaternion.identity, PalletLayer);
        return colliders.Select(p => p.GetComponentAnywhere<Pallet>())
            .Where(p => p != null)
            .OrderBy(p => (transform.position - p.transform.position).magnitude);
    }

    void Reset()
    {
        PalletLayer = LayerMask.GetMask("Pallets");
    }
}