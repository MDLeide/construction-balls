using System;
using Sirenix.OdinInspector;

[Serializable]
class TubeBlockLink
{
    [ShowInInspector]
    public ITubeProvider Origin;
    [ShowInInspector]
    public ITubeReceiver Destination;
    public float Length;

    public TubeBlockLink Copy()
    {
        return new TubeBlockLink()
        {
            Origin = Origin,
            Destination = Destination,
            Length = Length
        };
    }
}