using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


class FPS : MonoBehaviour
{
    const float MeasurePeriod = .5f;

    int _accumulator;
    float _nextPeriod;
    int _currentFps;

    public TMP_Text FPSText;

    public InputActionReference ToggleFPS;

    void Start()
    {
        _nextPeriod = Time.time + MeasurePeriod;
        ToggleFPS.action.performed += c => FPSText.enabled = !FPSText.enabled;
    }

    void Update()
    {
        _accumulator++;
        if (Time.time > _nextPeriod)
        {
            _currentFps = (int) (_accumulator / MeasurePeriod);
            _accumulator = 0;
            _nextPeriod += MeasurePeriod;
            FPSText.text = $"{_currentFps}";
        }
    }
}