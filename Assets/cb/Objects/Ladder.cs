using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using UnityEngine;


class Ladder : MonoBehaviour
{
    public PickUp PickUp;

    void Start()
    {
        if (PickUp != null)
        {
            PickUp.Placed += (sender, args) => enabled = !PickUp.IsOnPallet;
            PickUp.PickedUp += (sender, args) => enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        var ladderCoordinator = other.gameObject.GetComponentAnywhere<LadderCoordinator>();
        if (ladderCoordinator != null)
            ladderCoordinator.MadeContact();
    }

    void OnTriggerExit(Collider other)
    {
        if (!enabled)
            return;

        var ladderCoordinator = other.gameObject.GetComponentAnywhere<LadderCoordinator>();
        if (ladderCoordinator != null)
            ladderCoordinator.LeftContact();
    }
}