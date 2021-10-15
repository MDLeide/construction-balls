using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


class LadderController : MonoBehaviour
{
    Vector3 _moveDirection = Vector3.zero;

    public PlayerController PlayerController;

    public float ClimbSpeed;
    
    void Update()
    {
        SetMovement();

        PlayerController.CharacterController.Move(_moveDirection * Time.deltaTime);
        PlayerController.SetRotation();
    }


    void SetMovement()
    {
        var move = PlayerController.Move.action.ReadValue<Vector2>();
        
        float sideSpeed = ClimbSpeed * move.x;
        var right = transform.TransformDirection(Vector3.right);
        _moveDirection = right * sideSpeed;


        if (PlayerController.CharacterController.isGrounded && move.y < 0)
        {
            var forward = transform.TransformDirection(Vector3.forward);
            _moveDirection += forward * PlayerController.WalkSpeed * move.y;
        }
        else
        {
            float climbSpeed = PlayerController.WalkSpeed * move.y;
            _moveDirection.y = climbSpeed;
        }
    }
}
