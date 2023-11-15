using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 7.5f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private InputAction move;
    [SerializeField] private InputAction jump;
    [SerializeField] private InputAction sprint;

    [Header("Gravity")]
    [SerializeField] private float jumpHeight = 200f;
    [SerializeField] private float gravity = 9.82f;

    [Header("Angle")]
    [SerializeField] private float targetAngle;
    [SerializeField] private float angle;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float TurnSmoothVelocity;

    [Header("Object references")]
    [SerializeField] Transform Camera;
    [SerializeField] private CharacterController controller;


    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();

        sprint = playerControls.Player.Sprint;
        sprint.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        sprint.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get user input
        Vector2 Direction = move.ReadValue<Vector2>();
        float Jumping = jump.ReadValue<float>();
        float Sprinting = sprint.ReadValue<float>(); 

        Vector3 moveDirection = Vector3.zero;

        // movement
        if (Direction.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(Direction.x, Direction.y) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // find the moveSpeed
            Running(Sprinting);

            // Move
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
    }

    private void Running(float S)
    {
        if (S > 0)
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }



    }



    // Ground check
    private bool Grounded()
    {
        return Physics.Raycast(
            transform.position + controller.center, Vector3.down,
            controller.bounds.extents.y + controller.skinWidth + 0.05f);
    }
}
