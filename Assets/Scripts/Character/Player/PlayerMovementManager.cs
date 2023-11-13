using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerMovementManager : CharacterMovementManager
{
    CharacterController controller;
    PlayerManager player;
    public float verticalMovement;
    public float horizontalMovement;

    private Vector3 moveDirection;
    private float gravity;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float rotationSpeed = 15;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
        controller = GetComponent<CharacterController>();
    }


    public void HandleAllMovement()
    {
        Gravity();
        GroundedMovement();
        HandleRotation();
    }

    private void GetVerticalAndHorizontalInput()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
    }
    private bool Grounded()
    {
        return Physics.Raycast(player.transform.position + controller.center, Vector3.down, controller.bounds.extents.y + controller.skinWidth + 0.2f);
    }

    private float Gravity()
    {
        if (!Grounded() == true)
        {
            return gravity = -9.82f;
        }
        else
        {
            return gravity = 0;
        }
    }

    private void GroundedMovement()
    {
        GetVerticalAndHorizontalInput();
        
        // moving direction is based on the camera
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = gravity;
        

        if (PlayerInputManager.instance.moveAmount > 0.5f)
        {
            player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

}
