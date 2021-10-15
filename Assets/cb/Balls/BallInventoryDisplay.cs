using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

class BallInventoryDisplay : MonoBehaviour
{
    public BallInventory Inventory;

    [Space]
    public TMP_Text BlueText;
    public TMP_Text RedText;
    public TMP_Text YellowText;

    [Space]
    public TMP_Text GreenText;
    public TMP_Text PurpleText;
    public TMP_Text OrangeText;
    
    void Update()
    {
        if (Inventory == null)
        {
            SetError(BlueText);
            SetError(RedText);
            SetError(YellowText);

            SetError(GreenText);
            SetError(PurpleText);
            SetError(OrangeText);
            return;
        }

        Set(BlueText, Inventory.Blue);
        Set(RedText, Inventory.Red);
        Set(YellowText, Inventory.Yellow);

        Set(GreenText, Inventory.Green);
        Set(PurpleText, Inventory.Purple);
        Set(OrangeText, Inventory.Orange);
    }

    void Set(TMP_Text text, int val)
    {
        if (text != null)
            text.text = Format.Number(val);
    }

    void SetError(TMP_Text text)
    {
        if (text != null)
            text.text = "E:NO";
    }
}