using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


class Rotatable : MonoBehaviour
{
    public Interactable Interactable;
    public InputActionReference Reverse;
    public Vector3 RotationToApply = new Vector3(0, 90f, 0);

    void Start()
    {
        Interactable.Pushed += InteractableOnPushed;
    }

    void InteractableOnPushed(object sender, InteractablePushedEventArgs e)
    {
        var rot = RotationToApply;
        if (Reverse.action.ReadValue<float>() > 0)
            rot = -rot;
        transform.Rotate(rot);
    }
}