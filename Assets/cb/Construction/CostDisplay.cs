using TMPro;
using UnityEngine;

class CostDisplay : MonoBehaviour
{
    public BallCost Cost;

    public TMP_Text BlueCost;
    public TMP_Text RedCost;
    public TMP_Text YellowCost;

    void Update()
    {
        BlueCost.text = Format.Number(Cost.Blue);
        RedCost.text = Format.Number(Cost.Red);
        YellowCost.text = Format.Number(Cost.Yellow);
    }
}