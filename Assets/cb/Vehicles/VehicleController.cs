using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

class VehicleController : MonoBehaviour
{
    float _turn;

    public Rigidbody RigidBody;

    [Header("Settings")]

    public float MaxSpeed = 2;
    public float MaxReverseSpeed = 1;  
    [Space]
    public float Acceleration = 1;
    public float BrakePower = 1;
    public float SlowDown = .5f;
    [Space]
    public float TurnDegree = 45;

    [Header("Input")]
    public InputActionReference SteerAction;
    public InputActionReference AccelerationAction;
    public InputActionReference ExitVehicle;

    [Header("Monitor")]
    public float Speed;

    public EventHandler ExitRequested;

    void Start()
    {
        ExitVehicle.action.performed += c =>
        {
            if (enabled)
                ExitRequested?.Invoke(this, new EventArgs());
        };
    }

    void FixedUpdate()
    {
        var steer = SteerAction.action.ReadValue<float>();
        if (steer < 0)
            _turn -= TurnDegree * Time.deltaTime;
        else if (steer > 0)
            _turn += TurnDegree * Time.deltaTime;

        if (_turn < -360)
            _turn += 360;
        if (_turn > 360)
            _turn -= 360;

        var accel = AccelerationAction.action.ReadValue<float>();
        if (accel > float.Epsilon)
        {
            Speed += Acceleration * Time.deltaTime ;
            if (Speed > MaxSpeed)
                Speed = MaxSpeed;
        }
        else if (accel < -float.Epsilon)
        {
            Speed -= BrakePower * -accel * Time.deltaTime;

            if (Speed < -MaxReverseSpeed)
                Speed = -MaxReverseSpeed;
        }
        else
        {
            Speed -= SlowDown * Time.deltaTime;
            if (Speed < 0)
                Speed = 0;
        }

        var newPos = RigidBody.position + RigidBody.transform.forward * Speed * Time.deltaTime;
        RigidBody.MovePosition(newPos);
        RigidBody.rotation = Quaternion.Euler(0, _turn, 0);
    }
}