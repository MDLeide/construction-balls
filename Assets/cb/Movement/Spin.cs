using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;


class Spin : MonoBehaviour
{
    [Tooltip("Rotations per second")]
    public float Speed = 1;

    public Vector3 Axis = Vector3.up;

    void Update()
    {
        transform.RotateAround(transform.position, transform.TransformDirection(Axis), Speed * 360 * Time.deltaTime);
    }

    [Button]
    public void ConstructionBlock()
    {
        Speed = .02f;
    }
}