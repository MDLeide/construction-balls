using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;


class Navigable : MonoBehaviour
{
    public NavMeshAgent Agent;

    /// <summary>
    /// Moves in a straight line to a target.
    /// </summary>
    /// <param name="position"></param>
    public void MoveTo(Vector3 position)
    {
        
    }

    /// <summary>
    /// Uses a navmesh to pathfind to a target.
    /// </summary>
    /// <param name="position"></param>
    public void NavigateTo(Vector3 position)
    {
        Agent.SetDestination(position);
    }

    void Reset()
    {
        Agent = GetComponent<NavMeshAgent>();
    }
}