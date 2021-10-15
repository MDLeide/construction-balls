using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


class Tablet : MonoBehaviour
{
    public TabletViews Views;

    public GameObject HubView;
    public GameObject BlueprintsView;

    void Start()
    {
        Views.AddView(HubView, "HUB", "HUB");
        Views.AddView(BlueprintsView, "BP", "Blueprints");

        Views.SetCurrentView(HubView, "HUB");
    }
}