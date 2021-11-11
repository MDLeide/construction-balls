using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;


class CraftingFinishedEventArgs : EventArgs
{
    public CraftingFinishedEventArgs(CraftingRecipe recipe, GameObject result)
    {
        Recipe = recipe;
        Result = result;
    }

    public CraftingRecipe Recipe { get; }
    public GameObject Result { get; }
}


class CraftingStartedEventArgs : EventArgs
{
    public CraftingStartedEventArgs(CraftingRecipe recipe)
    {
        Recipe = recipe;
    }

    public CraftingRecipe Recipe { get; }
}

class CraftingStation : MonoBehaviour
{
    float _finishCraft;
    GameObject _hologramGameObject;
    int _itemIndex;

    [Header("Settings")]
    public bool UseInventory;

    public float CraftTimeMultiplier = 1;
    public List< string> AllowedGroups = new List<string>
    {
        "default"
    };
    [Space]
    public BallInventory Inventory;
    public ItemSpawner Spawner;

    [Space]
    public List<CraftingRecipe> AvailableRecipes;

    [Header("Graphics")]
    public Transform HologramPosition;
    public CostDisplay CostDisplay;
    public TMP_Text ItemNameText;
    public TMP_Text QuantityText;

    [Header("Buttons")]
    public Interactable NextItem;
    public Interactable PreviousItem;
    public Interactable CraftItem;

    [Space]
    public Interactable IncreaseQuantity;
    public Interactable DecreaseQuantity;

    [Header("State")]
    public int CraftQuantity;
    [ReadOnly]
    public bool IsCrafting;
    [ShowInInspector, ReadOnly]
    public CraftingRecipe SelectedRecipe => AvailableRecipes.Any() ? AvailableRecipes[_itemIndex] : null;

    public Func<bool> CanCraft { get; set; } = () => true;

    public event EventHandler<CraftingStartedEventArgs> StartedCrafting;
    public event EventHandler<CraftingFinishedEventArgs> FinishedCrafting;

    void Start()
    {
        AvailableRecipes = Crafting.Instance.GetAvailableCraftingRecipes(AllowedGroups).ToList();

        Crafting.Instance.NewRecipesAvailable += OnNewRecipesAvailable;
        
        UpdateSelectedItem();

        NextItem.Pushed += (sender, args) => Next();
        PreviousItem.Pushed += (sender, args) => Previous();
        CraftItem.Pushed += (sender, args) => Craft();
        IncreaseQuantity.Pushed += (sender, args) => Increase();
        DecreaseQuantity.Pushed += (sender, args) => Decrease();
    }

    void OnNewRecipesAvailable(object sender, CraftingAvailabilityEventArgs e)
    {
        foreach (var recipe in e.NewRecipes)
            if (!AvailableRecipes.Contains(recipe) && AllowedGroups.Contains(recipe.Group))
                AvailableRecipes.Add(recipe);
        UpdateSelectedItem();
    }

    void Update()
    {
        QuantityText.text = Format.Number(CraftQuantity);

        // finish crafting
        if (IsCrafting && _finishCraft <= Time.time)
        {
            var obj = Spawner.Spawn();
            IsCrafting = false;
            CraftQuantity--;
            if (CraftQuantity > 0)
                Craft();
            if (CraftQuantity < 0)
                CraftQuantity = 0;

            FinishedCrafting?.Invoke(this, new CraftingFinishedEventArgs(SelectedRecipe, obj));
        }
    }

    [Button]
    public void Craft()
    {
        if (IsCrafting)
            return;

        if (!CanCraft())
        {
            CraftQuantity = 0;
            return;
        }

        if (UseInventory && !Inventory.Pay(SelectedRecipe.Cost))
        {
            CraftQuantity = 0;
            return;
        }

        if (CraftQuantity == 0)
            CraftQuantity = 1;

        StartedCrafting?.Invoke(this, new CraftingStartedEventArgs(SelectedRecipe));
        IsCrafting = true;
        _finishCraft = Time.time + SelectedRecipe.CraftTime * CraftTimeMultiplier;
    }

    void Increase()
    {
        CraftQuantity++;
    }

    void Decrease()
    {
        if (CraftQuantity > 0)
            CraftQuantity--;
    }

    void Next()
    {
        if (IsCrafting)
            return;

        _itemIndex++;
        if (_itemIndex >= AvailableRecipes.Count)
            _itemIndex = 0;
        UpdateSelectedItem();
    }

    void Previous()
    {
        if (IsCrafting)
            return;

        _itemIndex--;
        if (_itemIndex < 0)
            _itemIndex = AvailableRecipes.Count - 1;
        UpdateSelectedItem();
    }

    void UpdateSelectedItem()
    {
        Destroy(_hologramGameObject);

        if (SelectedRecipe == null)
            return;

        if (SelectedRecipe.CraftPrototype.GetComponentAnywhere<Hologram>() != null)
        {
            _hologramGameObject = Instantiate(SelectedRecipe.CraftPrototype, HologramPosition);
            _hologramGameObject.GetComponentAnywhere<Hologram>().SwitchToHologram();
        }

        CostDisplay.Cost = SelectedRecipe.Cost;
        ItemNameText.text = SelectedRecipe.CraftPrototype.name;
        Spawner.ObjectToSpawn = SelectedRecipe.CraftPrototype;
    }
}