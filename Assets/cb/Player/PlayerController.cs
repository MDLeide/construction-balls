using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    Vector3 _moveDirection = Vector3.zero;
    float _rotationX = 0;
    bool _running;
    bool _jump;

    public CharacterController CharacterController;

    [Header("Movement Controls")]
    public InputActionReference Move;
    public InputActionReference Jump;
    public InputActionReference LookInput;

    [Header("Movement Settings")]
    public bool ToggleRun;
    public float WalkSpeed = 7.5f;
    public float RunSpeed = 11.5f;
    public float JumpSpeed = 8.0f;
    public float Gravity = 20.0f;

    [Header("Look")]
    [Tooltip("Transform to rotate according to look input.")]
    public Transform Look;
    public bool InvertLook = true;
    public float LookSpeed = 2.0f;
    public float LookXLimit = 45.0f;

    void Start()
    {
        Jump.action.performed += c =>
        {
            if (enabled)
                _jump = true;
        };

        CharacterController = GetComponent<CharacterController>();

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

        CharacterController.Move(_moveDirection * Time.deltaTime);

        SetRotation();
    }

    public void SetRotation()
    {
        var look = LookInput.action.ReadValue<Vector2>();
        _rotationX += look.y * LookSpeed * (InvertLook ? -1 : 1);
        _rotationX = Mathf.Clamp(_rotationX, -LookXLimit, LookXLimit);
        Look.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, look.x * LookSpeed, 0);
    }

    void SetMovement()
    {
        var forward = transform.TransformDirection(Vector3.forward);
        var right = transform.TransformDirection(Vector3.right);
        var move = Move.action.ReadValue<Vector2>();
        
        float curSpeedX = (_running ? RunSpeed : WalkSpeed) * move.y;
        float curSpeedY = (_running ? RunSpeed : WalkSpeed) * move.x;

        var y = _moveDirection.y;
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        _moveDirection.y = y;
    }

    void ApplyGravity()
    {
        if (!CharacterController.isGrounded)
            _moveDirection.y -= Gravity * Time.deltaTime;
    }

    void CheckJump()
    {
        if (_jump && CharacterController.isGrounded)
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