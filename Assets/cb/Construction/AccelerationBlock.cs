using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


class AccelerationBlock : MonoBehaviour
{
    static float _next;
    static bool _keySet;
    static int _index;

    bool _updateNext;

    bool _isKey;

    public MeshRenderer Ring1;
    public MeshRenderer Ring2;
    public MeshRenderer Ring3;

    public Material MaterialA;
    public Material MaterialB;

    public float Interval;

    public Interactable ReverseInteractable;

    void Start()
    {
        if (!_keySet)
        {
            _isKey = true;
            _keySet = true;
        }

        _index = -1;
        ReverseInteractable.Pushed += (s, e) => transform.Rotate(Vector3.up, 180);
    }

    void Update()
    {
        if (_updateNext && _isKey)
        {
            _next = Time.time + Interval;
            if (_index == 2)
                _next += Interval; 
            else if (_index == 3)
            {
                _next += Interval * 3;
                _index = -1;
            }

            _index++;
            _updateNext = false;
        }

        if (_next <= Time.time)
        {
            if (_isKey)
                _updateNext = true;

            switch (_index)
            {
                case 0:
                    Ring1.material = MaterialB;
                    break;
                case 1:
                    Ring2.material = MaterialB;
                    break;
                case 2:
                    Ring3.material = MaterialB;
                    break;
                case 3:
                    Ring1.material = MaterialA;
                    Ring2.material = MaterialA;
                    Ring3.material = MaterialA;
                    break;
            }
        }
    }
}