using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using UnityEngine;


class FenceBlock : MonoBehaviour
{
    public LinkingBlock LinkingBlock;
    public BallColor Color;
    public GameObject FencePrototype;

    void Start()
    {
        LinkingBlock.LinkMade += LinkMade;
        LinkingBlock.CanLinkTo = CanLinkTo;
    }

    bool CanLinkTo(LinkingBlock arg)
    {
        var fenceBlock = arg.GetComponentAnywhere<FenceBlock>();
        if (fenceBlock == null)
            return false;

        return fenceBlock.Color == Color;
    }

    void LinkMade(object sender, LinkEventArgs e)
    {
        var fence = Instantiate(FencePrototype);
        fence.transform.parent = transform;
        fence.transform.position = e.Link.MidPointBottom;

        fence.transform.localScale = 
            fence.transform.localScale.WithNewZ(
                e.Link.HorizontalDistance - 1);
        if (e.Link.IsRotated)
            fence.transform.RotateAround(fence.transform.position, Vector3.up, 90);

        e.Link.LinkBroken += (o, args) => Destroy(fence.gameObject);
    }
}