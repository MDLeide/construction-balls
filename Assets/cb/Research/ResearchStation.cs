using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

class ResearchStation : MonoBehaviour
{
    GameObject _hologramGameObject;
    int _itemIndex;

    public BallInventory Inventory;
    
    [Space]
    public List<ResearchItemWrapper> ResearchableItems;

    [Header("Graphics")]
    public Transform HologramPosition;
    public CostDisplay CostDisplay;
    public TMP_Text ItemNameText;
    public TMP_Text TimeRemainingText;

    [Header("Buttons")]
    public Interactable NextItem;
    public Interactable PreviousItem;
    public Interactable DoResearch;
    
    [ShowInInspector, ReadOnly]
    public ResearchItemWrapper SelectedResearch => ResearchableItems.Any() ? ResearchableItems[_itemIndex] : null;


    void Start()
    {
        ResearchableItems = global::Research.Instance.GetResearchable().ToList();

        global::Research.Instance.ItemResearched += (sender, args) =>
        {
            ResearchableItems = global::Research.Instance.GetResearchable().ToList();
            if (_itemIndex >= ResearchableItems.Count)
                _itemIndex--;
            UpdateSelectedItem();
        };

        Inventory.BallReceived += InventoryOnBallReceived;

        UpdateSelectedItem();

        NextItem.Pushed += (sender, args) => Next();
        PreviousItem.Pushed += (sender, args) => Previous();
        DoResearch.Pushed += (sender, args) => Research();

        // using this to display how many seconds each color ball will contribue to research
        CostDisplay.Cost = new BallCost()
        {
            Blue = 1,
            Red = 3,
            Yellow = 10,
        };
    }

    void InventoryOnBallReceived(object sender, BallInventoryChangedEventArgs e)
    {
        if (SelectedResearch != null)
            SelectedResearch.SecondsElapsed += GetSeconds(e.Color) * e.Change;
    }

    int GetSeconds(BallColor color)
    {
        switch (color)
        {
            case BallColor.Blue:
                return 1;
            case BallColor.Red:
                return 3;
            case BallColor.Yellow:
                return 10;
            case BallColor.Green:
                break;
            case BallColor.Purple:
                break;
            case BallColor.Orange:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }

        return 0;
    }

    void Update()
    {
        if (SelectedResearch != null)
        {
            SelectedResearch.SecondsElapsed += Time.deltaTime;
            var ts = TimeSpan.FromSeconds(SelectedResearch.ResearchItem.TotalSeconds - SelectedResearch.SecondsElapsed);
            TimeRemainingText.text = $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }
        else
        {
            TimeRemainingText.text = "--:--:--";
        }
    }


    [Button]
    public void Research()
    {
        if (SelectedResearch.SecondsElapsed >= SelectedResearch.ResearchItem.TotalSeconds)
        {
            global::Research.Instance.ResearchItem(SelectedResearch);
        }
    }
    
    void Next()
    {
        _itemIndex++;
        if (_itemIndex >= ResearchableItems.Count)
            _itemIndex = 0;
        UpdateSelectedItem();
    }

    void Previous()
    {
        _itemIndex--;
        if (_itemIndex < 0)
            _itemIndex = ResearchableItems.Count - 1;
        UpdateSelectedItem();
    }

    void UpdateSelectedItem()
    {
        Destroy(_hologramGameObject);

        if (SelectedResearch == null)
            return;

        _hologramGameObject = Instantiate(SelectedResearch.ResearchItem.DisplayHologram, HologramPosition).gameObject;
        _hologramGameObject.GetComponentAnywhere<Hologram>().SwitchToHologram();

        ItemNameText.text = SelectedResearch.ResearchItem.DisplayHologram.name;
    }
}