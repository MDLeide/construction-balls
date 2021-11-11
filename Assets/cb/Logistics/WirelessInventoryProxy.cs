using System;
using System.Collections.Generic;
using Cashew.Utility.Extensions;
using UnityEngine;

enum FindByDistanceOptions
{
    Closest,
    Farthest,
    Random
}

class WirelessInventoryProxy : InventoryProxy
{
    public float Range = 75;
    public FindByDistanceOptions FindOptions = FindByDistanceOptions.Closest;
    
    protected override void Start()
    {
        RegisterInventory(GetInventory());
        base.Start();
    }

    BallInventory GetInventory()
    {
        var hits = Physics.OverlapBox(transform.position, Vector3.one * Range);
        var invs = new List<BallInventory>();

        foreach (var hit in hits)
        {
            var inv = hit.GetComponentAnywhere<BallInventory>();
            if (inv != null)
                invs.Add(inv);
        }

        if (FindOptions == FindByDistanceOptions.Random)
            return invs.Choose();

        if (FindOptions == FindByDistanceOptions.Closest)
        {
            BallInventory closest = null;
            var distance = float.MaxValue;

            foreach (var inv in invs)
            {
                var d = (inv.transform.position - transform.position).magnitude;
                if (d < distance)
                {
                    closest = inv;
                    distance = d;
                }
            }

            return closest;
        }

        if (FindOptions == FindByDistanceOptions.Farthest)
        {
            BallInventory farthest = null;
            var distance = float.MinValue;

            foreach (var inv in invs)
            {
                var d = (inv.transform.position - transform.position).magnitude;
                if (d > distance)
                {
                    farthest = inv;
                    distance = d;
                }
            }

            return farthest;
        }

        throw new ArgumentOutOfRangeException();
    }
}