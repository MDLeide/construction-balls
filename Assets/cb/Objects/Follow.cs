using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;


class Follow : MonoBehaviour
{
    float _nextPositionUpdate;

    Vector3 _targetPosition;

    public Transform Target;
    public NavMeshAgent Agent;
    public float FollowDistance = 5;

    [Space]
    // the minimum distance between a previous position and a new one required to trigger a new pathfinding operation
    public float MinimumTargetChangeDistance = .5f;

    public float UpdateTargetFrequency = 5;

    void Start()
    {
        DoFollow();
    }

    void Update()
    {
        if (_nextPositionUpdate <= Time.time)
        {
            DoFollow();
            _nextPositionUpdate = Time.time + UpdateTargetFrequency;
        }
    }

    bool UpdateTargetPosition()
    {
        var dif = Target.position - transform.position;
        if (dif.magnitude < FollowDistance)
            return false;

        var p = (dif.magnitude - FollowDistance) / dif.magnitude;
        var delta = dif * p;
        var target = transform.position + delta;
        var min = (target - _targetPosition).magnitude >= MinimumTargetChangeDistance;
        _targetPosition = transform.position + delta;
        return min;
    }

    void DoFollow()
    {
        if (!UpdateTargetPosition())
            return;

        Agent.destination = _targetPosition;
    }

    void Reset()
    {
        Agent = GetComponent<NavMeshAgent>();
        if (Agent == null)
            Agent = gameObject.AddComponent<NavMeshAgent>();
    }
}