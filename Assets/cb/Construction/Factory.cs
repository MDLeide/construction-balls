using DigitalRuby.Tween;
using UnityEngine;

class Factory : MonoBehaviour
{
    float _finishCraft;

    [Header("Craft Settings")]
    public GameObject ItemToCraft;
    public BallCost CraftCost;
    public float CraftTime;

    [Header("References")]

    public BallInventory BallInventory;
    public Transform CraftPoint;
    public ParticleSystem Particles; 

    [Header("Monitor")]
    public bool IsCrafting;


    void Update()
    {
        if (IsCrafting)
        {
            if (Time.time >= _finishCraft)
            {
                var item = Instantiate(ItemToCraft);
                item.transform.position = CraftPoint.position;
                IsCrafting = false;

                Particles?.Stop(true);
            }

            return;
        }

        if (BallInventory.CanPay(CraftCost))
        {
            BallInventory.Pay(CraftCost);
            IsCrafting = true;

            Particles?.Play(true);

            _finishCraft = Time.time + CraftTime;
        }
    }
}

