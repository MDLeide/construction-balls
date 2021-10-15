using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController_orig : MonoBehaviour
{
    CharacterController _characterController;
    Vector3 _moveDirection = Vector3.zero;
    float _rotationX = 0;
    bool _running;
    bool _jump;

    [HideInInspector]
    public bool CanMove = true;

    [Header("Movement Controls")]
    public InputActionReference Move;
    public InputActionReference Jump;
    //public InputActionReference Run;
    public bool ToggleRun;
    public float WalkSpeed = 7.5f;
    public float RunSpeed = 11.5f;
    public float JumpSpeed = 8.0f;
    public float Gravity = 20.0f;

    [Header("Look")]
    [Tooltip("Transform to rotate according to look input.")]
    public Transform Look;
    public InputActionReference LookInput;
    public bool InvertLook = true;
    public float LookSpeed = 2.0f;
    public float LookXLimit = 45.0f;

    void Start()
    {
        //Run.action.performed += c => _running = true;
        Jump.action.performed += c => _jump = true;

        _characterController = GetComponent<CharacterController>();

        // Lock cursor
        if (Game.Instance.SwitchToGameMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        CheckRunning();
        SetMovement();
        CheckJump();
        ApplyGravity();
        _characterController.Move(_moveDirection * Time.deltaTime);
        SetRotation();
    }

    void SetRotation()
    {
        var look = LookInput.action.ReadValue<Vector2>();

        if (CanMove)
        {
            _rotationX += look.y * LookSpeed * (InvertLook ? -1 : 1);
            _rotationX = Mathf.Clamp(_rotationX, -LookXLimit, LookXLimit);
            Look.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, look.x * LookSpeed, 0);
        }
    }

    void SetMovement()
    {
        var forward = transform.TransformDirection(Vector3.forward);
        var right = transform.TransformDirection(Vector3.right);
        var move = Move.action.ReadValue<Vector2>();
        float curSpeedX = CanMove ? (_running ? RunSpeed : WalkSpeed) * move.y : 0;
        float curSpeedY = CanMove ? (_running ? RunSpeed : WalkSpeed) * move.x : 0;
        var y = _moveDirection.y;
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        _moveDirection.y = y;
    }

    void ApplyGravity()
    {
        if (!_characterController.isGrounded)
            _moveDirection.y -= Gravity * Time.deltaTime;
    }

    void CheckJump()
    {
        if (_jump && CanMove && _characterController.isGrounded)
            _moveDirection.y = JumpSpeed;

        _jump = false;
    }

    void CheckRunning()
    {
        if (ToggleRun)
        {
            if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
                _running = !_running;
        }
        else
        {
            _running = Keyboard.current.leftShiftKey.isPressed;
        }
    }
}