using Cashew.Utility.Async;
using Cinemachine;
using DigitalRuby.Tween;
using UnityEngine;

class PalletDriver : MonoBehaviour
{
    public Bob PalletBob;
    [Header("Transforms")]
    public Transform Player;
    public Transform StandPosition;
    [Header("Input")]
    public PlayerController PlayerController;
    public InputManager InputManager;
    public VehicleController VehicleController;
    public CharacterController CharacterController;
    [Header("Cameras")]
    public CinemachineVirtualCamera FirstPersonCamera;
    public CinemachineVirtualCamera VehicleCamera;

    [Header("Interactable")]
    public Interactable StartDrivingInteractable;

    void Start()
    {
        Player = Game.Instance.Player.transform;

        PlayerController = Game.Instance.Player.GetComponent<PlayerController>();
        InputManager = Game.Instance.Player.GetComponent<InputManager>();
        CharacterController = Game.Instance.Player.GetComponent<CharacterController>();

        FirstPersonCamera = Game.Instance.FirstPersonCamera;
        VehicleCamera = Game.Instance.DrivingCamera;

        StartDrivingInteractable.Pushed += (sender, args) => Drive();
        VehicleController.ExitRequested += (sender, args) => StopDriving();
    }

    public void Drive()
    {
        if (PalletBob != null)
            PalletBob.enabled = false;

        PlayerController.enabled = false;
        InputManager.enabled = false;
        
        FirstPersonCamera.enabled = false;
        VehicleCamera.enabled = true;

        CharacterController.enabled = false;

        Player.parent = transform;

        TweenFactory.Tween(
            new object(),
            Player.rotation,
            StandPosition.rotation,
            .25f,
            TweenScaleFunctions.CubicEaseInOut,
            p => Player.rotation = p.CurrentValue);

        TweenFactory.Tween(
            new object(),
            Player.position,
            StandPosition.position,
            .25f,
            TweenScaleFunctions.CubicEaseInOut,
            p => Player.position = p.CurrentValue,
            p =>
            {
                VehicleController.enabled = true;
            });

        VehicleController.enabled = true;
    }

    public void StopDriving()
    {
        if (PalletBob != null)
            PalletBob.enabled = true;

        FirstPersonCamera.enabled = true;
        VehicleCamera.enabled = false;

        _ = Execute.Later(
            .5f,
            () =>
            {
                Player.parent = null;
                PlayerController.enabled = true;
                InputManager.enabled = true;
                VehicleController.enabled = false;
                CharacterController.enabled = true;
            });

        
    }
}