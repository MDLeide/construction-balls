using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

class PlayerEyes : MonoBehaviour
{
    public LayerMask LayerMask;
    public Transform LookTransform;
    public float Range;

    [Header("In Sights")]
    [ReadOnly]
    public RaycastHit Hit;
    [ReadOnly]
    public GameObject HitObject;
    [Space]
    [ReadOnly]
    public Pallet PalletInSights;
    [ReadOnly]
    public PickUp PickupInSights;
    [ReadOnly]
    public Panel PanelInSights;
    [ReadOnly]
    public Block BlockInSights;
    [ReadOnly]
    public BuildingBlock BuildingBlockInSights;
    [ReadOnly]
    public Interactable InteractableInSights;
    
    void Update()
    {
        var ray = new Ray(LookTransform.position, LookTransform.forward);
        if (Physics.Raycast(ray, out var hit, Range, LayerMask))
        {
            HitObject = hit.transform.gameObject;

            PalletInSights = hit.collider.GetComponent<Pallet>();
            InteractableInSights = hit.collider.GetComponent<Interactable>();

            PickupInSights = hit.transform.gameObject.GetComponentAnywhere<PickUp>();
            BlockInSights = hit.transform.gameObject.GetComponentAnywhere<Block>();
            BuildingBlockInSights = hit.transform.gameObject.GetComponentAnywhere<BuildingBlock>();
            PanelInSights = hit.transform.gameObject.GetComponentAnywhere<Panel>();

            Hit = hit;
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