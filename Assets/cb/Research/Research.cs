using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

class Research : MonoBehaviour
{
    const string AssetPath = "Assets/cb/Research/Research Items";

    public static Research Instance;

    public EventHandler<ResearchEventArgs> ItemResearched;

    public ResearchItem[] AllResearch;

    [ReadOnly]
    public Dictionary<ResearchItem, ResearchItemWrapper> ResearchWrappers;
    
    void Start()
    {
        Instance = this;

        AllResearch = GetResearchItems();

        ResearchWrappers = AllResearch.Select(p => new ResearchItemWrapper() {ResearchItem = p})
            .ToDictionary(k => k.ResearchItem, v => v);

        foreach (var research in AllResearch)
        {
            if (research.ID <= 0 && Game.Instance.WarnOnInvalidID)
                Debug.LogWarning($"Research has invalid ID: {research.name}");
            if (research.DisplayHologram == null && Game.Instance.WarnOnMissingHologram)
                Debug.LogWarning($"Research is missing hologram: {research.name}");
        }

        // auto research test group stuff
        foreach (var kvp in ResearchWrappers)
        {
            if (kvp.Key.Group == "test")
                ResearchItem(kvp.Value);
        }
    }

    public void ResearchItem(ResearchItemWrapper item)
    {
        if (!item.IsResearched)
        {
            item.IsResearched = true;
            ItemResearched?.Invoke(this, new ResearchEventArgs(item.ResearchItem));
            ActivityLog.Log($"Research has completed: <color=orange>{item.ResearchItem.name}</color>");
        }
    }

    public void ResearchAll()
    {
        foreach (var item in ResearchWrappers.Values)
            ResearchItem(item);
    }

    public void ResearchItem(int id)
    {
        var item = ResearchWrappers.Values.FirstOrDefault(p => p.ResearchItem.ID == id);
        if (item != null)
            ResearchItem(item);
        else
            Debug.Log($"Could not find research with ID: {id}");
    }

    public void ResearchItem(string name)
    {
        var item = ResearchWrappers.Values.FirstOrDefault(p => NamesMatch(p.ResearchItem.name, name));
        if (item != null)
            ResearchItem(item);
        else
            Debug.Log($"Could not find research named: {name}");

        bool NamesMatch(string a, string b)
        {
            return string.Equals(a.Replace(" ", ""), b.Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public IEnumerable<ResearchItemWrapper> GetResearchable()
    {
        return ResearchWrappers.Select(p => p.Value)
            .Where(
                p => !p.IsResearched && (p.ResearchItem.ResearchRequired == null ||
                                         IsResearched(p.ResearchItem.ResearchRequired)));
    }

    public bool IsResearched(ResearchItem research)
    {
        return ResearchWrappers[research].IsResearched;
    }

    public bool IsResearched(ResearchItemWrapper research)
    {
        return research.IsResearched;
    }

    ResearchItem[] GetResearchItems()
    {
        return AssetDatabaseHelper.LoadAssetsFromFolder<ResearchItem>(AssetPath);
    }
}