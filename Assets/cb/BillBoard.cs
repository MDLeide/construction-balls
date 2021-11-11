using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class BillBoard : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Game.Instance.Player.Look.position);
    }
}