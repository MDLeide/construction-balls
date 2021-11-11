using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;


class PBlockLink : MonoBehaviour
{
    [Header("State")]
    [ReadOnly]
    public PBlock A;
    [ReadOnly]
    public PBlock B;
    [ReadOnly]
    public Link Link;

    [Header("Prototypes")]
    public GameObject StraightPipe;
    public GameObject[] InclinePipes;
    
    void Start()
    {
        A = Link.Creator.GetComponentAnywhere<PBlock>();
        B = Link.OtherBlock.GetComponentAnywhere<PBlock>();

        if (A == null || B == null)
            throw new InvalidOperationException("Pipe block link must have two valid pipe blocks");

        CreatePipe();
    }

    public void CreatePipe()
    {
        // subtract 2 from distance
        // 1 because each pipe block has .5 extra pipe on each connected side
        // another 1 since we are measuring from the center of the pipeblocks
        var hDist = Link.HorizontalDistance - 2;
        if (hDist <= 0)
            return;

        var pipe = CreatePipe(Link.VerticalDistance);

        pipe.transform.position = Link.MidPointBottom;
        pipe.transform.localScale = GetScale(pipe, hDist, Link.VerticalDistance);

        if (Link.IsRotated)
            pipe.transform.Rotate(Vector3.up, 90f);

        // flip inclined pipes to line up correctly
        if (Link.VerticalDistance > 0 && ShouldFlip(Link.IsRotated))
            pipe.transform.Rotate(Vector3.up, 180f);
    }

    Vector3 GetScale(GameObject pipe, int horizontalDistance, int verticalDistance)
    {
        if (verticalDistance > 0)
            return pipe.transform.localScale.WithNewZ(horizontalDistance / (float)verticalDistance);

        return pipe.transform.localScale = pipe.transform.localScale.WithNewZ(horizontalDistance);
    }

    GameObject CreatePipe(int verticalDistance)
    {
        return verticalDistance == 0
            ? Instantiate(StraightPipe, transform)
            : Instantiate(InclinePipes[verticalDistance - 1], transform);
    }

    bool ShouldFlip(bool rotate)
    {
        if (A.transform.position.y > B.transform.position.y)
        {
            if (rotate)
                return A.transform.position.x < B.transform.position.x;

            return A.transform.position.z < B.transform.position.z;
        }
        else
        {
            if (rotate)
                return B.transform.position.x < A.transform.position.x;

            return B.transform.position.z < A.transform.position.z;
        }
    }

    int GetVerticalDistance()
    {
        return (int)(Mathf.Abs(A.transform.position.y - B.transform.position.y) /
                      Game.UnitDistance);
    }

    int GetHorizontalDistance(out bool rotate)
    {
        rotate = true;
        var horizontalDistance = (int)(Mathf.Abs(A.transform.position.x - B.transform.position.x) /
                                        Game.UnitDistance);
        if (horizontalDistance == 0)
        {
            horizontalDistance = (int)(Mathf.Abs(A.transform.position.z - B.transform.position.z) /
                                     Game.UnitDistance);
            rotate = false;
        }

        // subtract 2 from distance
        // 1 because each pipe block has .5 extra pipe on each connected side
        // another 1 since we are measuring from the center of the pipeblocks
        horizontalDistance -= 2;

        return horizontalDistance;
    }
}