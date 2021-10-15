using System.Collections.Generic;
using System.Linq;
using Cashew.Utility.Extensions;
using UnityEngine;

class Hologram : MonoBehaviour
{
    [Header("Graphics")]
    public bool ApplyScaleChange = true;
    public Vector3 CraftHologramScale = new Vector3(.5f, .5f, .5f);
    
    [Header("Components to Disable")]
    public Rigidbody Rigidbody;
    public Collider[] Colliders;
    public MonoBehaviour[] ComponentsToDisable;
    public GameObject[] GameObjectsToDisable;

    public bool HologramOnStartup;

    void Start()
    {
        if (HologramOnStartup)
            SwitchToHologram();
    }

    public void SwitchToHologram()
    {
        if (Rigidbody != null)
            Rigidbody.isKinematic = true;

        foreach (var collider in Colliders)
            collider.enabled = false;

        foreach (var comp in ComponentsToDisable)
            comp.enabled = false;

        foreach (var go in GameObjectsToDisable)
            go.SetActive(false);

        if (ApplyScaleChange)
            transform.localScale = CraftHologramScale;
    }

    void Reset()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Colliders = this.GetComponentsAnywhere<Collider>().ToArray();

        var components = new List<MonoBehaviour>();
        var pickup = this.GetComponentAnywhere<PickUp>();
        var interactable = this.GetComponentAnywhere<Interactable>();
        var block = this.GetComponentAnywhere<Block>();
        
        if (pickup != null)
            components.Add(pickup);
        if (interactable != null)
            components.Add(interactable);
        if (block != null)
            components.Add(block);

        ComponentsToDisable = components.ToArray();
    }
}