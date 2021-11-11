using System;
using Cashew.Utility.Extensions;
using UnityEngine;

class ConnectionBlock : MonoBehaviour
{
    public LinkingBlock LinkingBlock;

    public ConnectionLink LinkA;
    public ConnectionLink LinkB;

    public event EventHandler LinkMade;
    public event EventHandler LinkBroken;
    public event EventHandler ChainUpdated; 

    void Start()
    {
        LinkingBlock.LinkMade += OnLinkMade;
    }

    public ConnectionBlock GetOtherSide()
    {
        if (LinkA != null && LinkB != null)
            throw new InvalidOperationException(
                "Cannot call GetOtherSide on a connection block that is not at the end of the chain.");
        
        if (LinkA != null)
            return GetSideA();
        
        if (LinkB != null)
            return GetSideB();
        
        return this;
    }

    public ConnectionBlock GetSideA()
    {
        if (LinkA == null)
            return this;
        return LinkA.GetOtherSide(this).GetSideA();
    }

    public ConnectionBlock GetSideB()
    {
        if (LinkB == null)
            return this;
        return LinkB.GetOtherSide(this).GetSideB();
    }

    void OnLinkMade(object sender, LinkEventArgs e)
    {
        if (LinkA != null && LinkB != null)
            return;

        var connectionBlock = e.Link.OtherBlock.GetComponentAnywhere<ConnectionBlock>();
        if (connectionBlock == null)
            return;

        // other side has no connections available
        if (connectionBlock.LinkA != null && connectionBlock.LinkB != null)
            return;

        var isLinkA = LinkA == null;
        var link = MakeLink();

        if (isLinkA)
            LinkA = link;
        else
            LinkB = link;

        var otherIsA = connectionBlock.LinkA == null;

        if (otherIsA)
            connectionBlock.LinkA = link;
        else
            connectionBlock.LinkB = link;
        
        e.Link.LinkBroken += OnLinkBroken;

        LinkMade?.Invoke(this, new EventArgs());
        connectionBlock.LinkMade?.Invoke(this, new EventArgs());

        UpdateChain();

        ConnectionLink MakeLink()
        {
            var connectionLink = new ConnectionLink();
            connectionLink.Creator = this;
            connectionLink.Other = connectionBlock;
            return connectionLink;
        }

        void OnLinkBroken(object s, EventArgs ev)
        {
            if (isLinkA)
                LinkA = null;
            else
                LinkB = null;

            if (otherIsA)
                connectionBlock.LinkA = null;
            else
                connectionBlock.LinkB = null;

            LinkBroken?.Invoke(this, new EventArgs());
            connectionBlock.LinkBroken?.Invoke(this, new EventArgs());
            UpdateChain();
            connectionBlock.UpdateChain();
        }
    }

    void UpdateChain()
    {
        ChainUpdated?.Invoke(this, new EventArgs());
        LinkA?.GetOtherSide(this).UpdateChain();
        LinkB?.GetOtherSide(this).UpdateChain();
    }
}