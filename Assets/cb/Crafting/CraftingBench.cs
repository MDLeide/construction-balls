using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;


class CraftingBench : MonoBehaviour
{
    Pulse _pulse;
    GameObject _toCraft;
    float _finishCraft;

    [Header("General")]
    public float CraftTime;
    public Transform CraftPoint;
    [ReadOnly]
    public bool IsCrafting;
    [Space]
    public BallInventory BallInventory;

    [Header("Graphics")]
    public Transform SphereTransform;

    [Header("Building Blocks")]
    public GameObject BlueBuildingBlock;
    public GameObject RedBuildingBlock;
    public GameObject YellowBuildingBlock;

    [Header("Costs")]
    public BallCost BlueCost;
    public BallCost RedCost;
    public BallCost YellowCost;

    [Header("Buttons")]
    public Interactable BlueButton;
    public Interactable RedButton;
    public Interactable YellowButton;

    void Start()
    {
        BlueButton.InteractedWith += (s, e) => OnBlueButton();
        RedButton.InteractedWith += (s, e) => OnRedButton();
        YellowButton.InteractedWith += (s, e) => OnYellowButton();
        _pulse = new Pulse();
        _pulse.Transform = SphereTransform;
    }

    void Update()
    {
        if (IsCrafting && _finishCraft <= Time.time)
        {
            _pulse.StopPulsing();
            Instantiate(_toCraft, CraftPoint.position, Quaternion.identity);
            IsCrafting = false;
        }
    }

    void OnBlueButton()
    {
        Craft(BlueCost, BlueBuildingBlock);
    }

    void OnRedButton()
    {
        Craft(RedCost, RedBuildingBlock);
    }

    void OnYellowButton()
    {
        Craft(YellowCost, YellowBuildingBlock);
    }

    void Craft(BallCost cost, GameObject toCraft)
    {
        if (BallInventory.Pay(cost))
        {
            _pulse.StartPulsing();
            IsCrafting = true;
            _toCraft = toCraft;
            _finishCraft = Time.time + CraftTime;
        }
    }
}
