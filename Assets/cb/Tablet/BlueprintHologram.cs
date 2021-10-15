using UnityEngine;

class BlueprintHologram : MonoBehaviour
{
    public static BlueprintHologram Instance;

    public GameObject CurrentHologram;

    void Start()
    {
        Instance = this;
    }

    public void SetBuilding(Building building)
    {
        // todo: add a building hologram in addition to the layout
        Destroy(CurrentHologram);
        CurrentHologram = Instantiate(building.LayoutHologramPrototype, transform);
    }
}