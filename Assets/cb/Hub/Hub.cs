using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


class Hub : MonoBehaviour
{
    public static Hub Instance;

    public HubStats Stats;
    public ResearchStation Research;

    void Start()
    {
        Instance = this;
    }
}