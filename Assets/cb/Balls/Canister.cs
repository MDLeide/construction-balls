using Sirenix.OdinInspector;
using UnityEngine;



class Canister : MonoBehaviour
{
    public BallColor Color;
    public int MaxQuantity;
    [Space]
    [ReadOnly]
    public int Quantity;

    public Transform FillTransform;

    void Start()
    {
        UpdateFill();
    }

    public bool CanAdd()
    {
        return MaxQuantity > Quantity;
    }

    [Button]
    public bool Add()
    {
        if (Quantity < MaxQuantity)
        {
            Quantity++;
            UpdateFill();
            return true;
        }

        return false;
    }

    [Button]
    public int Subtract()
    {
        return Subtract(1);
    }
    
    public int Subtract(int quantity)
    {
        if (Quantity < quantity)
        {
            quantity -= Quantity;
            Quantity = 0;
            UpdateFill();
            return quantity;
        }

        Quantity -= quantity;
        UpdateFill();
        return 0;
    }

    void UpdateFill()
    {
        // i dont understand blender exports/imports so we have weird scaling across weird axes
        FillTransform.localScale = new Vector3(
            100,
            100,
            Quantity / (float) MaxQuantity * 100);
    }
}