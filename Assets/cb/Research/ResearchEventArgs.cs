using System;

class ResearchEventArgs : EventArgs
{
    public ResearchEventArgs(ResearchItem research)
    {
        Research = research;
    }

    public ResearchItem Research { get; }
}