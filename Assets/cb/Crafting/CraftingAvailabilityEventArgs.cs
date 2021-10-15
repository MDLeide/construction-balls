using System;
using System.Collections.Generic;
using UnityEngine;

class CraftingAvailabilityEventArgs : EventArgs
{
    public CraftingAvailabilityEventArgs(IEnumerable<CraftingRecipe> newRecipes)
    {
        NewRecipes = newRecipes;
    }

    public IEnumerable<CraftingRecipe> NewRecipes { get; }
}