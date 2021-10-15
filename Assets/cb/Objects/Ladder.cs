using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using UnityEngine;


class Ladder : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var ladderCoordinator = other.gameObject.GetComponentAnywhere<LadderCoordinator>();
        if (ladderCoordinator != null)
            ladderCoordinator.MadeContact();
    }

    void OnTriggerExit(Collider other)
    {
        var ladderCoordinator = other.gameObject.GetComponentAnywhere<LadderCoordinator>();
        if (ladderCoordinator != null)
            ladderCoordinator.LeftContact();
    }
}