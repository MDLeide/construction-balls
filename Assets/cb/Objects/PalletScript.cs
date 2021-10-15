using System;
using Cashew.Utility.Async;
using DigitalRuby.Tween;
using UnityEngine;

class PalletScript : MonoBehaviour
{
    int _phase;

    public float PalletSpeed;

    public ItemSpawnerController ControllerA;
    public ItemSpawnerController ControllerB;
    public ItemSpawnerController ControllerC;

    public int UnitsPerPallet;

    [Header("Pallets")]
    public Transform PalletA1;
    public Transform PalletB1;
    public Transform PalletC1; 
    [Space]
    public Transform PalletA2;
    public Transform PalletB2;
    public Transform PalletC2;
    
    [Header("Positions")]
    public Transform LoadA;
    public Transform LoadB;
    public Transform LoadC;
    [Space]
    public Transform DepartA1;
    public Transform DepartB1;
    public Transform DepartC1;
    [Space]
    public Transform DepartA2;
    public Transform DepartB2;
    public Transform DepartC2;

    void Start()
    {
        ControllerA.ObjectsToCraft = UnitsPerPallet;
        ControllerB.ObjectsToCraft = UnitsPerPallet;
        ControllerC.ObjectsToCraft = UnitsPerPallet;
    }

    void Update()
    {
        if (ControllerA.ObjectsCrafted >= ControllerA.ObjectsToCraft)
        {
            if (_phase == 0)
            {
                _ = Execute.Later(
                    0.1f,
                    () =>
                    {
                        SendAway(PalletA1, DepartA1);
                        SendAway(PalletB1, DepartB1);
                        SendAway(PalletC1, DepartC1);
                    });

                _ = Execute.Later(
                    1f,
                    () =>
                    {
                        Send(PalletA2, LoadA);
                        Send(PalletB2, LoadB);
                        Send(
                            PalletC2,
                            LoadC,
                            () =>
                            {
                                ControllerA.ObjectsCrafted = 0;
                                ControllerB.ObjectsCrafted = 0;
                                ControllerC.ObjectsCrafted = 0;
                                _phase++;
                            });
                    });

                _phase++;
            }
            else if (_phase == 2)
            {
                SendAway(PalletA2, DepartA2);
                SendAway(PalletB2, DepartB2);
                SendAway(PalletC2, DepartC2);
                _phase++;
            }
        }
    }

    void SendAway(Transform pallet, Transform target)
    {
        var distance = (pallet.position - target.position).magnitude;
        TweenFactory.Tween(
            new object(),
            pallet.position,
            target.position,
            distance / PalletSpeed,
            TweenScaleFunctions.QuadraticEaseIn,
            p => pallet.position = p.CurrentValue);
    }

    void Send(Transform pallet, Transform target, Action onComplete = null)
    {
        var dist = (pallet.position - target.position).magnitude;
        TweenFactory.Tween(
            new object(),
            pallet.position,
            target.position,
            dist / PalletSpeed,
            TweenScaleFunctions.QuadraticEaseInOut,
            p => pallet.position = p.CurrentValue,
            p => onComplete?.Invoke());
    }
}