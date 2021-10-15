using Cashew.Utility.Extensions;
using DigitalRuby.Tween;
using Sirenix.OdinInspector;
using UnityEngine;

class Bob : MonoBehaviour
{
    bool _reverse;

//    float _traveled;

    public float Distance = 1;
    public float Speed = 1;

    void Start()
    {
        var end = transform.localPosition.WithNewY(y => y + Distance * (_reverse ? -1 : 1));

        var offset = Distance * Random.value;
        if (Random.value < .5)
            offset = -offset;

        var start = transform.localPosition.WithNewY(y => y + offset);

        var dist = (end - start).magnitude;
        
        TweenFactory.Tween(
            this,
            start,
            end,
            dist / Speed,
            TweenScaleFunctions.QuadraticEaseInOut,
            p =>
            {
                if (this != null && enabled)
                    transform.localPosition = p.CurrentValue;
            },
            p =>
            {
                _reverse = !_reverse;

                if (this != null)
                    DoBob();
            });
    }

    void DoBob()
    {
        TweenFactory.Tween(
            this,
            transform.localPosition,
            transform.localPosition.WithNewY(y => y + Distance * 2 * (_reverse ? -1 : 1)),
            Distance * 2 / Speed,
            TweenScaleFunctions.QuadraticEaseInOut,
            p =>
            {
                if (this != null && enabled)
                    transform.localPosition = p.CurrentValue;
            },
            p =>
            {
                _reverse = !_reverse;
                if (this != null)
                    DoBob();
            });
    }

    [Button]
    public void ConstructionBlock()
    {
        Distance = .05f;
        Speed = .05f;
    }
}