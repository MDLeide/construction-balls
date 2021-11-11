using System;
using UnityEngine;

class ResearchStationInventoryConnector : MonoBehaviour
{
    public BallInventory Inventory;
    public ResearchStation ResearchStation;

    [Header("Bonus Time")]
    public float Blue;
    public float Red;
    public float Yellow;
    public float Orange;
    public float Green;
    public float Purple;

    void Start()
    {
        Inventory.BallReceived += BallReceived;
    }

    void BallReceived(object sender, BallInventoryChangedEventArgs e)
    {
        if (ResearchStation.SelectedResearch != null)
            ResearchStation.SelectedResearch.SecondsElapsed += GetSeconds(e.Color);
    }

    float GetSeconds(BallColor color)
    {
        switch (color)
        {
            case BallColor.Blue:
                return Blue;
            case BallColor.Red:
                return Red; 
            case BallColor.Yellow:
                return Yellow;
            case BallColor.Green:
                return Green;
            case BallColor.Purple:
                return Purple;
            case BallColor.Orange:
                return Orange;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }
}