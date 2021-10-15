using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Data/Recipe", order = 1)]
class CraftingRecipe : ScriptableObject
{
    public int ID;
    public ResearchItem ResearchRequired;
    public GameObject CraftPrototype;
    public float CraftTime;
    public BallCost Cost;
}

