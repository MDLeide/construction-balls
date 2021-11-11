using UnityEngine;

class Building : ScriptableObject
{
    public int ID;

    [Header("Blueprint")]
    public Blueprint Blueprint;
    public ResearchItem RequiredResearch; 
    
    [Header("Prototypes")]
    public GameObject BuildingHologramPrototype;
    public GameObject LayoutHologramPrototype;
    public GameObject BuildingPrototype;

    [Header("Offsets")]
    public Vector3 BuildingHologramOffsetFromKeyBlock;
    public Vector3 LayoutOffsetFromKeyBlock;
    public Vector3 BuildingOffsetFromKeyBlock;

    public string GetBuildingName()
    {
        return name.Replace(" - Building", string.Empty);
    }
}