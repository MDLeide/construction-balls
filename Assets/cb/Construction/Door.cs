using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using DigitalRuby.Tween;
using UnityEngine;


class Door : MonoBehaviour
{
    Vector3 _outLeftOrig;
    Vector3 _inLeftOrig;
    Vector3 _outRightOrig;
    Vector3 _inRightOrig;

    public float MoveDistance;
    public float OpenTime;
    public float CloseTime;

    [Header("Left")]
    public Transform InnerLeft; 
    public Transform OuterLeft;
    
    [Header("Right")]
    public Transform InnerRight; 
    public Transform OuterRight;

    void Start()
    {
        _outLeftOrig = OuterLeft.localPosition;
        _outRightOrig = OuterRight.localPosition;
        _inLeftOrig = InnerLeft.localPosition;
        _inRightOrig = InnerRight.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
            Open();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
            Close();
    }

    void Open()
    {
        TweenFactory.Tween(
            new object(),
            InnerLeft.transform.localPosition,
            InnerLeft.transform.localPosition.WithNewX(x => x + MoveDistance * 2),
            OpenTime / 2,
            TweenScaleFunctions.QuadraticEaseIn,
            p => InnerLeft.transform.localPosition = p.CurrentValue,
            p =>
            {
                
            });

        TweenFactory.Tween(
            new object(),
            OuterLeft.transform.localPosition,
            OuterLeft.transform.localPosition.WithNewX(x => x + MoveDistance),
            OpenTime / 2,
            TweenScaleFunctions.Linear,
            ip => OuterLeft.transform.localPosition = ip.CurrentValue);

        TweenFactory.Tween(
            new object(),
            InnerRight.transform.localPosition,
            InnerRight.transform.localPosition.WithNewX(x => x - MoveDistance * 2),
            OpenTime / 2,
            TweenScaleFunctions.QuadraticEaseIn,
            p => InnerRight.transform.localPosition = p.CurrentValue,
            p =>
            {
                
            });

        TweenFactory.Tween(
            new object(),
            OuterRight.transform.localPosition,
            OuterRight.transform.localPosition.WithNewX(x => x - MoveDistance),
            OpenTime / 2,
            TweenScaleFunctions.Linear,
            ip => OuterRight.transform.localPosition = ip.CurrentValue);
    }

    void Close()
    {
        TweenFactory.Tween(
            new object(),
            OuterLeft.transform.localPosition,
            _outLeftOrig,
            CloseTime,
            TweenScaleFunctions.QuadraticEaseIn,
            p => OuterLeft.transform.localPosition = p.CurrentValue);

        TweenFactory.Tween(
            new object(),
            InnerLeft.transform.localPosition,
            _inLeftOrig,
            CloseTime,
            TweenScaleFunctions.QuadraticEaseIn,
            p => InnerLeft.transform.localPosition = p.CurrentValue);

        TweenFactory.Tween(
            new object(),
            OuterRight.transform.localPosition,
            _outRightOrig,
            CloseTime,
            TweenScaleFunctions.QuadraticEaseIn,
            p => OuterRight.transform.localPosition = p.CurrentValue);

        TweenFactory.Tween(
            new object(),
            InnerRight.transform.localPosition,
            _inRightOrig,
            CloseTime,
            TweenScaleFunctions.QuadraticEaseIn,
            p => InnerRight.transform.localPosition = p.CurrentValue);
    }
}