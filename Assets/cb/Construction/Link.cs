using System;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

class Link : MonoBehaviour
{
    public LineRenderer LineRenderer;
    [Space]
    [ReadOnly]
    public LinkingBlock Creator;
    [ReadOnly]
    public LinkingBlock OtherBlock;

    public Vector3 MidPoint =>
        Creator.transform.position +
        (OtherBlock.transform.position - Creator.transform.position) / 2;

    public Vector3 MidPointBottom => MidPoint.WithNewY(Mathf.Min(Creator.transform.position.y, OtherBlock.transform.position.y));
    
    public bool IsRotated => Mathf.Abs(OtherBlock.transform.position.x - Creator.transform.position.x) > 0;

    public int HorizontalDistance
    {
        get
        {
            var d = Mathf.Abs(Creator.transform.position.x - OtherBlock.transform.position.x);
            if (Math.Abs(d) < float.Epsilon)
                d = Mathf.Abs(Creator.transform.position.z - OtherBlock.transform.position.z);
            return Mathf.RoundToInt(d / Game.UnitDistance);
        }
    }

    public int VerticalDistance =>
        (int) (Mathf.Abs(Creator.transform.position.y - OtherBlock.transform.position.y) / Game.UnitDistance);


    public event EventHandler LinkBroken;


    void Start()
    {
        LineRenderer.positionCount = 2;
        LineRenderer.SetPositions(new Vector3[]
        {
            Creator.Block.MidPoint,
            OtherBlock.Block.MidPoint
        });

        LineRenderer.startWidth = .2f;
        LineRenderer.endWidth = .05f;
    }

    public void OnLinkBroken()
    {
        LinkBroken?.Invoke(this, new EventArgs());
    }

    public LinkingBlock Other(LinkingBlock caller)
    {
        if (Creator == caller)
            return OtherBlock;

        if (OtherBlock == caller)
            return Creator;

        throw new InvalidOperationException();
    }
}