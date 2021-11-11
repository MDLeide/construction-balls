using System;

class LinkEventArgs : EventArgs
{
    public LinkEventArgs(Link link)
    {
        Link = link;
    }

    public Link Link { get; }
    
}