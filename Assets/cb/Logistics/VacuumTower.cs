using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class VacuumTower : MonoBehaviour
{
    public PipeBlock PipeBlock;
    public Pipe Pipe;

    void Update()
    {
        Pipe.enabled = PipeBlock.IsConnected;
    }
}