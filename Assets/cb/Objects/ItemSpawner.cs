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
    Pallet _pallet;

    int _count;

    public GameObject ObjectToSpawn;
    public bool TryToPlaceOnPallet = true;
    public float PalletRange = 10;
    public LayerMask PalletLayer;

    public GameObject Spawn()
    {
        var obj = Instantiate(ObjectToSpawn, transform.position, transform.rotation);
        obj.name += $" [{++_count}]";

        var block = obj.GetComponentAnywhere<Block>();
        if (block != null)
            if (block.IdText != null)
                block.IdText.text = _count.ToString();

        if (!TryToPlaceOnPallet)
            return obj;

        var pickup = obj.GetComponentAnywhere<PickUp>();
        if (pickup == null)
            return obj;

        if (_pallet != null && PalletIsValid(_pallet) && _pallet.CanPlace(pickup))
        {
            _pallet.Place(pickup);
            return obj;
        }

        _pallet = null;
        var pallets = GetPalletsInRange();
        foreach (var pallet in pallets)
        {
            if (pallet.Place(pickup))
            {
                _pallet = pallet;
                return obj;
            }
        }

        return obj;
    }

    bool PalletIsValid(Pallet pallet)
    {
        return (pallet.transform.position - transform.position).magnitude <= PalletRange;
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