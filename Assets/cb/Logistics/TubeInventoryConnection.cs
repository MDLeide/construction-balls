using Cashew.Utility.Extensions;
using UnityEngine;

class TubeInventoryConnection : MonoBehaviour
{
    public TubeTerminator Terminator;
    public BallInventory Inventory;

    void Start()
    {
        Terminator.BallReceived += TerminatorOnBallReceived;
        Terminator.CanReceiveFunc = CanReceiveFunc;
    }

    bool CanReceiveFunc(BallColor color)
    {
        return Inventory.CanAdd(color);
    }

    void TerminatorOnBallReceived(object sender, TubeTerminatorEventArgs e)
    {
        if (Inventory.Add(e.Color))
            e.Handled = true;
    }

    void Reset()
    {
        Terminator = this.GetComponentAnywhere<TubeTerminator>();
        Inventory = this.GetComponentAnywhere<BallInventory>();
    }
}