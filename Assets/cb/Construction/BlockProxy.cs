using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class BlockProxy : MonoBehaviour
{
    bool _sticky;
    Vector3 _target;

    public PickUp PickUp;

    void Start()
    {
        PickUp.BeingPlaced += PickUpOnBeingPlaced;
        PickUp.Placed += PickUpOnPlaced;
        PickUp.PickedUp += PickUpOnPickedUp;
    }

    void Update()
    {
        if (_sticky)
            transform.position = _target;
    }

    void PickUpOnPickedUp(object sender, EventArgs e)
    {
        _sticky = false;
    }

    void PickUpOnPlaced(object sender, EventArgs e)
    {
        _sticky = false;
    }

    void PickUpOnBeingPlaced(object sender, PickUpPlacingEventArgs e)
    {
        _sticky = true;
        _target = e.TargetPosition;
    }

}