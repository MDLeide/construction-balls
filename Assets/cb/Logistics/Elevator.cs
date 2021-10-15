using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class Elevator : MonoBehaviour
{
    const float SpeedMultiplier = .65f;
    public List<Animator> Animators;
    public float Speed = 1;
    
    void Start()
    {
        var interval = 1f / Animators.Count;

        for (int i = 0; i < Animators.Count; i++)
        {
            Animators[i].SetFloat("offset", interval * i);
            Animators[i].speed = Speed * SpeedMultiplier;
        }
    }
}