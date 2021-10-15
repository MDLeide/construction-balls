using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


class Clock : MonoBehaviour
{
    public TMP_Text TimeText;

    void Update()
    {
        var ts = TimeSpan.FromSeconds(Time.time);
        TimeText.text = ts.ToString(@"hh\:mm\:ss");
    }
}