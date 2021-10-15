using System;
using UnityEngine;

class BallCollector : MonoBehaviour
{
    public BallInventory Inventory;

    void OnTriggerEnter(Collider other)
    {
        var ball = other.gameObject.GetComponent<Ball>();
        if (ball == null)
            ball = other.gameObject.GetComponentInParent<Ball>();
        if (ball == null)
            return;

        if (Inventory.Add(ball))
            Destroy(ball.gameObject);
    }
}