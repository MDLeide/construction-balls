using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

class PlayerEyes : MonoBehaviour
{
    [Header("Debug")]
    public bool DrawTargetedCell = true;

    [Header("Settings")]
    public LayerMask LayerMask;
    public Transform LookTransform;
    public float Range;

    [Header("In Sights")]
    public RaycastHit Hit;
    public GameObject HitObject;
    public GameObject CollidedObject;
    [Space]
    public Pallet PalletInSights;
    public PickUp PickupInSights;
    public Panel PanelInSights;
    public Block BlockInSights;
    public BuildingBlock BuildingBlockInSights;
    public Interactable InteractableInSights;
    
    void Update()
    {
        var ray = new Ray(LookTransform.position, LookTransform.forward);
        if (Physics.Raycast(ray, out var hit, Range, LayerMask))
        {
            HitObject = hit.transform.gameObject;
            CollidedObject = hit.collider.gameObject;

            PalletInSights = hit.transform.GetComponent<Pallet>();
            InteractableInSights = hit.collider.GetComponent<Interactable>();

            PickupInSights = hit.transform.gameObject.GetComponentAnywhere<PickUp>();
            BlockInSights = hit.transform.gameObject.GetComponentAnywhere<Block>();
            BuildingBlockInSights = hit.transform.gameObject.GetComponentAnywhere<BuildingBlock>();
            PanelInSights = hit.transform.gameObject.GetComponentAnywhere<Panel>();

            Hit = hit;

            if (DrawTargetedCell)
                DrawHelper.DrawBox(GridHelper.GetCenterPointOfCell(hit.point), Game.HalfUnitCube, Color.blue);
        }
        else
        {
            HitObject = null;

            PickupInSights = null;
            PalletInSights = null;
            InteractableInSights = null;
            BlockInSights = null;
            BuildingBlockInSights = null;
            PanelInSights = null;
        }
    }
}