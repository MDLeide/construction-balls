using System;
using Cashew.Utility.Extensions;
using UnityEngine;

[Obsolete]
class PipeBlockConnection : MonoBehaviour
{
    public bool DisplayLineRenderer = true;

    public PipeBlock A;
    public PipeBlock B;
    public LineRenderer LineRenderer;

    public GameObject StraightPipe;
    public GameObject[] InclinePipes;

    public bool IsBuilt;
    // true if this connection could be made between the two blocks, but
    // has not yet. some graphic is drawn to display the link
    public bool IsPotential => !IsBuilt;

    void Update()
    {
        LineRenderer.enabled = DisplayLineRenderer;
    }

    public void CreatePipe()
    {
        var verticalDistance = GetVerticalDistance();
        var horizontalDistance = GetHorizontalDistance(out var rotate);

        if (horizontalDistance <= 0)
            return;

        var pipe = CreatePipe(verticalDistance);
        
        pipe.transform.position = GetMidPoint();
        pipe.transform.localScale = GetScale(pipe, horizontalDistance, verticalDistance);

        if (rotate)
            pipe.transform.Rotate(Vector3.up, 90f);

        // flip inclined pipes to line up correctly
        if (verticalDistance > 0 && Flip(rotate))
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

    bool Flip(bool rotate)
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
        return (int) (Mathf.Abs(A.transform.position.y - B.transform.position.y) /
                      Game.UnitDistance);
    }
    
    int GetHorizontalDistance(out bool rotate)
    {
        rotate = true;
        var horizontalDistance = (int) (Mathf.Abs(A.transform.position.x - B.transform.position.x) /
                                        Game.UnitDistance);
        if (horizontalDistance == 0)
        {
            horizontalDistance = (int) (Mathf.Abs(A.transform.position.z - B.transform.position.z) /
                                     Game.UnitDistance);
            rotate = false;
        }

        // subtract 2 from distance
        // 1 because each pipe block has .5 extra pipe on each connected side
        // another 1 since we are measuring from the center of the pipeblocks
        horizontalDistance -= 2;

        return horizontalDistance;
    }

    Vector3 GetMidPoint()
    {
        // i have no idea what is going on here...

        var magnitude = (B.transform.position - A.transform.position).magnitude;
        var a = A.transform.position +
                (B.transform.position - A.transform.position) * (A.RadiusInWorldUnits / magnitude);

        var b = B.transform.position +
                (A.transform.position - B.transform.position) * (B.RadiusInWorldUnits / magnitude);

        return new Vector3(
            (a.x + b.x) / 2,
            Mathf.Min(A.transform.position.y, B.transform.position.y),
            (a.z + b.z) / 2);
    }

    public void TurnOnLineRenderer()
    {
        LineRenderer.positionCount = 2;
        LineRenderer.SetPositions(new Vector3[]
        {
            A.transform.position.WithNewY(y => y + Game.UnitDistance / 2),
            B.transform.position.WithNewY(y => y + Game.UnitDistance / 2)
        });

        LineRenderer.startWidth = .2f;
        LineRenderer.endWidth = .05f;

        LineRenderer.enabled = true;
    }
}