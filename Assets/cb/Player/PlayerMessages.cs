using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


class PlayerMessages : MonoBehaviour
{
    public PlayerEyes Eyes;
    public PlayerHands Hands;

    [Header("Prompts")]
    public TMP_Text GrabMessage;

    public TMP_Text PlaceMessage;
    public TMP_Text TakeMessage;
    public TMP_Text InteractMessage;
    
    void Update()
    {
        GrabMessage.enabled = false;
        PlaceMessage.enabled = false;
        TakeMessage.enabled = false;
        InteractMessage.enabled = false;

        //if (Eyes.BuildingBlockInSights != null && Hands.HeldObject == null)
        //{
        //    if (Eyes.BuildingBlockInSights.CanBeBuilt)
        //        BuildMessage.enabled = true;
        //    GrabMessage.enabled = true;
        //}


        if (Hands.CanInteract())
        {
            InteractMessage.enabled = true;
            InteractMessage.text = Eyes.InteractableInSights.Message;
        }
        else if (Eyes.PalletInSights != null)
        {
            if (Hands.CanPlaceOnPallet())
            {
                PlaceMessage.enabled = true;
                PlaceMessage.text = Eyes.PalletInSights.PlaceMessage;
            }
            else if (Hands.CanTake())
            {
                TakeMessage.enabled = true;
                TakeMessage.text = Eyes.PalletInSights.TakeMessage;
            }
        }
        else if (Hands.CanGrab())
        {
            GrabMessage.enabled = true;
        }
    }
}