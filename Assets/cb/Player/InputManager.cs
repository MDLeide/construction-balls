using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandTerminal;
using UnityEngine;
using UnityEngine.InputSystem;


class InputManager : MonoBehaviour
{
    float _throwStart;
    public float MaxThrowTime;

    public PlayerHands Hands;
    public PlayerController PlayerController;
    public Terminal Terminal;
    public Tablet Tablet;

    [Header("Input")]
    public InputActionReference GrabFromTop;
    public InputActionReference GrabAction; // exclusive with: interact
    public InputActionReference ReleaseAction;
    public InputActionReference ThrowAction;
    [Space]
    public InputActionReference PlaceAction;
    public InputActionReference TakeAction;
    [Space]
    public InputActionReference InteractAction;
    [Space]
    public InputActionReference ToggleTablet;

    void Start()
    {
        ToggleTablet.action.performed += c =>
        {
            var turnOn = !Tablet.gameObject.activeSelf;

            if (turnOn)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;

            Tablet.gameObject.SetActive(turnOn);

            PlayerController.enabled = !turnOn;
            Hands.enabled = !turnOn;
        };

        Terminal.Opened += (sender, args) =>
        {
            PlayerController.enabled = false;
        };

        Terminal.Closed += (sender, args) =>
        {
            PlayerController.enabled = true;
        };
        
        GrabAction.action.performed += c =>
        {
            if (enabled)
            {
                if (GrabFromTop.action.ReadValue<float>() > 0)
                    Hands.GrabFromTop();
                else
                    Hands.Grab();
            }
        };

        ReleaseAction.action.performed += c =>
        {
            if (enabled)
                Hands.Release();
        };
        
        PlaceAction.action.performed += c =>
        {
            if (!enabled)
                return;

            if (Hands.CanPlaceOnPallet())
                Hands.PlaceOnPallet();
            else if (Hands.CanPlaceOnGround())
                Hands.PlaceOnGround();
            else if (Hands.CanPlaceOnBlock())
                Hands.PlaceOnBlock();
        };

        TakeAction.action.performed += c =>
        {
            if (enabled)
                Hands.Take();
        };

        InteractAction.action.performed += c =>
        {
            if (enabled)
                Hands.Interact();
        };
    }

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            _throwStart = Time.time;
        }

        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            var t = (Time.time - _throwStart) / MaxThrowTime;
            if (t > 1)
                t = 1;
            Hands.Throw(Hands.ThrowPowerMin + (Hands.ThrowPowerMax - Hands.ThrowPowerMin) * t);
        }
    }
}