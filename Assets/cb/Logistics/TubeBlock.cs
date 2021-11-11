using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using UnityEngine;


class TubeBlock : MonoBehaviour
{
    public LinkingBlock LinkingBlock;
    public TubePort Port;
    public ITubeProvider Source;
    public ITubeReceiver Destination;
    
    void Start()
    {
        LinkingBlock.LinkMade += OnLinkMade;
        LinkingBlock.CanLinkTo = CanLinkTo;
    }

    bool CanLinkTo(LinkingBlock block)
    {
        if (Source != null && Destination != null)
            return false;

        var origin = block.GetComponentAnywhere<TubeOrigin>();
        if (origin != null)
            return origin.Port.Outbound?.Destination == null;

        var term = block.GetComponentAnywhere<TubeTerminator>();
        if (term != null)
            return term.Port.Inbound?.Origin == null;

        var tube = block.GetComponentAnywhere<TubeBlock>();
        // two connections already
        if (tube.Source != null && tube.Destination != null)
            return false;

        // we can pass to this tube
        if (Source == null && tube.Destination == null)
            return true;

        // we can receive from this tube
        if (Destination == null && tube.Source == null)
            return true;

        // no connection
        return false;
    }

    void OnLinkMade(object sender, LinkEventArgs e)
    {
        var origin = e.Link.OtherBlock.GetComponentAnywhere<TubeOrigin>();
        if (origin != null)
        {
            Port.Inbound = new TubeBlockLink();
            Port.Inbound.Origin = origin;
            Port.Inbound.Destination = Port;
            Port.Inbound.Length = Mathf.Abs((e.Link.OtherBlock.transform.position - transform.position).magnitude);

            origin.Port.Outbound = Port.Inbound.Copy();

            Source = origin;

            e.Link.LinkBroken += (o, args) =>
            {
                Port.Inbound = null;
                origin.Port.Outbound = null;
                Source = null;
            };

            return;
        }

        var termination = e.Link.OtherBlock.GetComponentAnywhere<TubeTerminator>();
        if (termination != null)
        {
            Port.Outbound = new TubeBlockLink();
            Port.Outbound.Origin = Port;
            Port.Outbound.Destination = termination;
            Port.Outbound.Length = Mathf.Abs((e.Link.OtherBlock.transform.position - transform.position).magnitude);

            termination.Port.Inbound = Port.Outbound.Copy();

            Destination = termination;

            e.Link.LinkBroken += (o, args) =>
            {
                Port.Outbound = null;
                termination.Port.Inbound = null;
                Destination = null;
            };

            return;
        }

        var tubeBlock = e.Link.OtherBlock.GetComponentAnywhere<TubeBlock>();
        if (tubeBlock == null)
            throw new InvalidOperationException();

        if (tubeBlock.Source == null && Destination == null)
        {
            Port.Outbound = new TubeBlockLink();
            Port.Outbound.Origin = Port;
            Port.Outbound.Destination = tubeBlock.Port;
            Port.Outbound.Length = Mathf.Abs((tubeBlock.transform.position - transform.position).magnitude);

            tubeBlock.Port.Inbound = Port.Outbound.Copy();

            Destination = tubeBlock.Port;
            tubeBlock.Source = Port; 
            
            e.Link.LinkBroken += (o, args) =>
            {
                Port.Outbound = null;
                tubeBlock.Port.Inbound = null;
                Destination = null;
                tubeBlock.Source = null;
            };

            return;
        }

        if (tubeBlock.Destination == null && Source == null)
        {
            Port.Inbound = new TubeBlockLink();
            Port.Inbound.Origin = tubeBlock.Port;
            Port.Inbound.Destination = Port;
            Port.Inbound.Length = Mathf.Abs((tubeBlock.transform.position - transform.position).magnitude);

            tubeBlock.Port.Outbound = Port.Inbound.Copy();

            Source = tubeBlock.Port;
            tubeBlock.Destination = Port;

            e.Link.LinkBroken += (o, args) =>
            {
                Port.Inbound = null;
                tubeBlock.Port.Outbound = null;
                Source = null;
                tubeBlock.Destination = null;
            };
           
            return;
        }

        throw new InvalidOperationException();
    }


}