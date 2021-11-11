using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Crafting : MonoBehaviour
{
    const string AssetPath = "Assets/cb/Crafting/Recipes";

    public static Crafting Instance;
    public CraftingRecipe[] AllRecipes;
    
    public event EventHandler<CraftingAvailabilityEventArgs> NewRecipesAvailable;

    void Start()
    {
        Instance = this;

        AllRecipes = AssetDatabaseHelper.LoadAssetsFromFolder<CraftingRecipe>(AssetPath);

        foreach (var recipe in AllRecipes)
        {
            if (recipe.ID <= 0 && Game.Instance.WarnOnInvalidID)
                Debug.LogWarning($"Crafting recipe has invalid ID: {recipe.name}");
        }

        Research.Instance.ItemResearched += Researched;
    }

    void Researched(object sender, ResearchEventArgs e)
    {
        var newRecipes = AllRecipes.Where(p => p.ResearchRequired == e.Research).ToArray();

        if (newRecipes.Any())
            NewRecipesAvailable?.Invoke(
            this,
            new CraftingAvailabilityEventArgs(newRecipes));
    }

    public IEnumerable<CraftingRecipe> GetAvailableCraftingRecipes(IEnumerable<string> allowedGroups)
    {
        return GetAvailableCraftingRecipes().Where(p => allowedGroups.Contains(p.Group));
    }


    public IEnumerable<CraftingRecipe> GetAvailableCraftingRecipes()
    {
        return AllRecipes.Where(p => Research.Instance.IsResearched(p.ResearchRequired));
    }
}