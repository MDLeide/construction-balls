using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Openable : MonoBehaviour
{
    int _frame;

    public PickUp PickUp;
    public Interactable Interactable;

    public GameObject Unopened;
    public GameObject Opened;

    public List<MonoBehaviour> Disable;

    void Start()
    {
        Interactable.Pushed += (sender, args) => Open();
        PickUp.PickedUp += (sender, args) => Close();
    }

    void Update()
    {
        if (_frame == Time.frameCount)
        {
            Opened.SetActive(true);
        }
    }

    void Open()
    {
        if (!PickUp.IsPlaced)
            return;

        Unopened.SetActive(false);
        foreach (var c in Disable)
            c.enabled = false;

        _frame = Time.frameCount + 2;
    }

    void Close()
    {
        Unopened.SetActive(true);
        Opened.SetActive(false);

        foreach (var c in Disable)
            c.enabled = true;
    }

    void Reset()
    {
        PickUp = GetComponent<PickUp>();
        if (PickUp == null)
            PickUp = gameObject.AddComponent<PickUp>();

        Interactable = GetComponentInChildren<Interactable>();

        Unopened = transform.Find("unopened")?.gameObject;
        if (Unopened == null)
        {
            Unopened =new GameObject("unopened");
            Unopened.transform.parent = transform;
        }

        Opened = transform.Find("opened")?.gameObject;
        if (Opened == null)
        {
            Opened = new GameObject("opened");
            Opened.transform.parent = transform;
        }
    }
}