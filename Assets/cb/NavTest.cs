using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;


class NavTest : MonoBehaviour
{
    public NavMeshAgent Agent;
    public Transform Destination;

    void Start()
    {
        Agent.SetDestination(Destination.position);
    }

    void Reset()
    {
        Agent = GetComponent<NavMeshAgent>();
    }
}