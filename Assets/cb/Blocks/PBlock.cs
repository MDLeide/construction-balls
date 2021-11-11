using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;


class PBlock : MonoBehaviour
{
    [Header("Config")]
    public PBlockLink PipeBlockLinkPrototype;
    [Space]
    public GameObject StraightPipe;
    public GameObject CornerPipe;
    public GameObject NoPipe;
    [Space]
    public PickUp PickUp;
    public LinkingBlock LinkingBlock;

    public bool West => LinkingBlock.HasLink(CompassDirection.West);
    public bool East => LinkingBlock.HasLink(CompassDirection.East);
    public bool North => LinkingBlock.HasLink(CompassDirection.North);
    public bool South => LinkingBlock.HasLink(CompassDirection.South);

    public bool IsConnected => LinkingBlock.IsLinked;

    void Start()
    {
        if (transform.parent != null && !PickUp.IsPlacing)
        {
            if (!PickUp.IsPlaced && !PickUp.IsLocked)
                Debug.LogWarning($"{name} is a pipe block that has a parent, but the pick up is not placed or locked.");
            else if (!PickUp.IsPlaced)
                Debug.LogWarning($"{name} is a pipe block that has a parent, but the pick up is not placed.");
            else if (!PickUp.IsLocked)
                Debug.LogWarning($"{name} is a pipe block that has a parent, but the pick up is not locked.");

            if (!PickUp.RB.isKinematic)
                Debug.LogWarning($"{name} is a pipe block that has a parent, but its RB is not set to kinematic.");
        }

        LinkingBlock.LinkMade += OnLinkMade;
        LinkingBlock.LinkReceived += OnLinkReceived;
    }
    
    void OnLinkReceived(object sender, LinkEventArgs e)
    {
        e.Link.LinkBroken += (o, args) => UpdatePipeType();
        UpdatePipeType();
    }

    void OnLinkMade(object sender, LinkEventArgs e)
    {
        var pipeLink = Instantiate(PipeBlockLinkPrototype, transform);
        pipeLink.Link = e.Link;

        e.Link.LinkBroken += (o, args) =>
        {
            Destroy(pipeLink.gameObject);
            UpdatePipeType();
        };

        UpdatePipeType();
    }

    public virtual void UpdatePipeType()
    {
        StraightPipe.SetActive(false);
        CornerPipe.SetActive(false);
        NoPipe.SetActive(false);
        
        if (LinkingBlock.Links.Count == 1)
        {
            StraightPipe.SetActive(true);

            if (West || East)
                StraightPipe.transform.localRotation = Quaternion.Euler(0, 90, 0);
            else
                StraightPipe.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (North && South)
        {
            StraightPipe.SetActive(true);
            StraightPipe.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (West && East)
        {
            StraightPipe.SetActive(true);
            StraightPipe.transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (West && North)
        {
            CornerPipe.SetActive(true);
            CornerPipe.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (North && East)
        {
            CornerPipe.SetActive(true);
            CornerPipe.transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (East && South)
        {
            CornerPipe.SetActive(true);
            CornerPipe.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (South && West)
        {
            CornerPipe.SetActive(true);
            CornerPipe.transform.localRotation = Quaternion.Euler(0, -90, 0);
        }
        else
        {
            NoPipe.SetActive(true);
        }
    }

    void Reset()
    {
        LinkingBlock = GetComponent<LinkingBlock>();
        PickUp = GetComponent<PickUp>();

        StraightPipe = transform.Find("straight")?.gameObject;
        if (StraightPipe == null)
        {
            StraightPipe = new GameObject("straight");
            StraightPipe.transform.parent = transform;
        }

        CornerPipe = transform.Find("corner")?.gameObject;
        if (CornerPipe == null)
        {
            CornerPipe = new GameObject("corner");
            CornerPipe.transform.parent = transform;
        }

        NoPipe = transform.Find("pipe block")?.gameObject;
    }
}