using TMPro;
using UnityEngine;
using UnityEngine.UI;

class BlueprintsView : MonoBehaviour
{
    public TabletButton SelectBuildingPrototype;

    public VerticalLayoutGroup BuildingsList;
    public TMP_Text SelectedBuildingNameText;
    public RawImage BlueprintImage;

    void Start()
    {
        Construction.Instance.NewBuildingAvailable += (sender, args) => AddBuilding(args.NewBuilding);
        foreach (var bldg in Construction.Instance.AvailableBuildings)
            AddBuilding(bldg);
    }

    void AddBuilding(Building building)
    {
        var btn = Instantiate(SelectBuildingPrototype);
        btn.ButtonText.text = building.GetBuildingName();
        btn.Button.onClick.AddListener(
            () =>
            {
                SelectBuilding(building);
            });

        if (btn.transform is RectTransform rt)
            rt.SetParent(BuildingsList.transform);
    }

    void SelectBuilding(Building building)
    {
        BlueprintHologram.Instance.SetBuilding(building);
        SelectedBuildingNameText.text = building.GetBuildingName();
    }
}