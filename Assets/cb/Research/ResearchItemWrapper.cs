using System;

[Serializable]
class ResearchItemWrapper
{
    public ResearchItem ResearchItem;
    public bool IsResearched;
    public float SecondsElapsed;

    public bool IsFinished => SecondsElapsed >= ResearchItem.TotalSeconds;
}