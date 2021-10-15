using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Crafting : MonoBehaviour
{
    public static Crafting Instance;

    void Start()
    {
        Instance = this;

        Research.Instance.ItemResearched += Researched;
    }

    void Researched(object sender, ResearchEventArgs e)
    {
        NewRecipesAvailable?.Invoke(
            this,
            new CraftingAvailabilityEventArgs(AllCraftables.Where(p => p.ResearchRequired == e.Research)));
    }

    public CraftingRecipe[] AllCraftables;

    public event EventHandler<CraftingAvailabilityEventArgs> NewRecipesAvailable; 

    public IEnumerable<CraftingRecipe> GetAvailableCraftingRecipes()
    {
        return AllCraftables.Where(p => Research.Instance.IsResearched(p.ResearchRequired));
    }
}