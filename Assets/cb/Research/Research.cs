using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

class Research : MonoBehaviour
{
    public static Research Instance;

    public EventHandler<ResearchEventArgs> ItemResearched;

    public ResearchItem[] AllResearch;

    [ReadOnly]
    public Dictionary<ResearchItem, ResearchItemWrapper> ResearchWrappers;
    
    void Start()
    {
        Instance = this;

        ResearchWrappers = AllResearch.Select(p => new ResearchItemWrapper() {ResearchItem = p})
            .ToDictionary(k => k.ResearchItem, v => v);
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
        return ResearchWrappers.Select(p => p.Value).Where(p => !p.IsResearched);
    }

    public bool IsResearched(ResearchItem research)
    {
        return ResearchWrappers[research].IsResearched;
    }

    public bool IsResearched(ResearchItemWrapper research)
    {
        return research.IsResearched;
    }
}