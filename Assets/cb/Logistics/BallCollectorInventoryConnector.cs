using Cashew.Utility.Extensions;
using UnityEngine;

class BallCollectorInventoryConnector : MonoBehaviour
{
    public BallCollector BallCollector;
    public BallInventory Inventory;

    void Start()
    {
        BallCollector.BallCollected += OnBallCollected;
    }

    void OnBallCollected(object sender, BallCollectedEventArgs e)
    {
        if (Inventory.Add(e.Color))
            e.Handled = true;
    }

    void Reset()
    {
        BallCollector = this.GetComponentAnywhere<BallCollector>();
        Inventory = this.GetComponentAnywhere<BallInventory>();
    }
}