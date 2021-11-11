using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


class ResearchDisplay : MonoBehaviour
{
    float _nextBlink;

    public ResearchStation ResearchStation;
    public TMP_Text ResearchNameText;
    public TMP_Text ResearchTimeText;

    void Update()
    {
        if (ResearchStation != null)
        {
            if (ResearchStation.SelectedResearch != null)
            {
                ResearchNameText.text = ResearchStation.SelectedResearch.ResearchItem.name;

                if (ResearchStation.SelectedResearch.SecondsElapsed >= ResearchStation.SelectedResearch.ResearchItem.TotalSeconds)
                {
                    ResearchTimeText.text = "READY";

                    if (_nextBlink <= Time.time)
                    {
                        ResearchTimeText.enabled = !ResearchTimeText.enabled;
                        if (ResearchTimeText.enabled)
                            _nextBlink = Time.time + 2f;
                        else
                            _nextBlink = Time.time + .25f;
                    }
                }
                else
                {
                    ResearchTimeText.enabled = true;
                    var ts = TimeSpan.FromSeconds(ResearchStation.SelectedResearch.ResearchItem.TotalSeconds - 
                                                  ResearchStation.SelectedResearch.SecondsElapsed);

                    ResearchTimeText.text = $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
                }
            }
            else
            {
                ResearchNameText.text = "No Research";
                ResearchTimeText.enabled = true;
                ResearchTimeText.text = "--:--:--";
            }
        }
        else
        {
            ResearchNameText.text = "No Research";
        }
    }
}