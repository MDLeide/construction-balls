using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEngine;

class ResearchStation : MonoBehaviour
{
    GameObject _hologramGameObject;
    int _itemIndex;
    float _nextBlink;

    [Header("Settings")]
    public float LabRange = 25;

    public List<string> AllowedGroups = new List<string> {"default"};

    [Header("Graphics")]
    public Transform HologramPosition;
    public TMP_Text ItemNameText;
    public TMP_Text TimeRemainingText;

    [Header("Buttons")]
    public Interactable NextItem;
    public Interactable PreviousItem;
    public Interactable DoResearch;
    
    [ShowInInspector, ReadOnly]
    public ResearchItemWrapper SelectedResearch => ResearchableItems.Any() ? ResearchableItems[_itemIndex] : null;

    [ShowInInspector, ReadOnly]
    public List<ResearchLab> ConnectedLabs { get; } = new List<ResearchLab>();

    [ShowInInspector, ReadOnly]
    public List<ResearchItemWrapper> ResearchableItems { get; private set; } = new List<ResearchItemWrapper>();


    void Start()
    {
        NextItem.CanPush = () => ResearchableItems.Count > 1;
        NextItem.Pushed += (sender, args) => Next();

        PreviousItem.CanPush = () => ResearchableItems.Count > 1;
        PreviousItem.Pushed += (sender, args) => Previous();

        DoResearch.CanPush = () => SelectedResearch != null && SelectedResearch.IsFinished;
        DoResearch.Pushed += (sender, args) => FinishResearch();

        Research.Instance.ItemResearched += ItemResearched;

        UpdateResearchableItems();
        UpdateSelectedItem();
        UpdateLabs();
    }
    
    void Update()
    {
        if (SelectedResearch != null)
        {
            if (SelectedResearch.SecondsElapsed >= SelectedResearch.ResearchItem.TotalSeconds)
            {
                TimeRemainingText.text = "READY";

                if (_nextBlink <= Time.time)
                {
                    TimeRemainingText.enabled = !TimeRemainingText.enabled;
                    if (TimeRemainingText.enabled)
                        _nextBlink = Time.time + 2f;
                    else
                        _nextBlink = Time.time + .25f;
                }
            }
            else
            {
                TimeRemainingText.enabled = true;
                SelectedResearch.SecondsElapsed += Time.deltaTime;
                var ts = TimeSpan.FromSeconds(SelectedResearch.ResearchItem.TotalSeconds - SelectedResearch.SecondsElapsed);
                TimeRemainingText.text = $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
            }
        }
        else
        {
            TimeRemainingText.enabled = true;
            TimeRemainingText.text = "--:--:--";
        }
    }

    [Button]
    public void FinishResearch()
    {
        if (SelectedResearch != null && SelectedResearch.IsFinished)
            Research.Instance.ResearchItem(SelectedResearch);
    }

    public void UpdateLabs()
    {
        var colliders = Physics.OverlapBox(transform.position, new Vector3(LabRange, LabRange, LabRange));
        foreach (var collider in colliders)
        {
            var lab = collider.transform.GetComponentAnywhere<ResearchLab>();
            if (lab != null)
                lab.UpdateStationsInRange();
        }
    }

    void ItemResearched(object sender, ResearchEventArgs e)
    {
        UpdateResearchableItems();

        if (_itemIndex >= ResearchableItems.Count)
            _itemIndex--;

        UpdateSelectedItem();
    }

    void UpdateResearchableItems()
    {
        ResearchableItems = Research.Instance.GetResearchable()
            .Where(p => AllowedGroups.Contains(p.ResearchItem.Group))
            .ToList();
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
        {
            ItemNameText.text = "No Research Available";
            return;
        }

        ItemNameText.text = SelectedResearch.ResearchItem.name;

        if (SelectedResearch.ResearchItem.DisplayHologram != null)
        {
            _hologramGameObject = Instantiate(SelectedResearch.ResearchItem.DisplayHologram, HologramPosition).gameObject;
            _hologramGameObject.GetComponentAnywhere<Hologram>().SwitchToHologram();
        }
    }
}