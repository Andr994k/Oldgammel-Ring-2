using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private InputAction move;
    [SerializeField] private InputAction sprint;
    [SerializeField] private InputAction space;
    [SerializeField] private bool Shift_pressed;
    [SerializeField] private bool Space_pressed;

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float rollTime;
    [SerializeField] private float backstepSpeed = 5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] Vector3 moveDirection = Vector3.zero;
    [SerializeField] public bool rollInvincibility;

    [Header("Gravity")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = -9.82f;
    [SerializeField] Vector3 gravityDirection = Vector3.zero;

    [Header("Angle")]
    [SerializeField] private float targetAngle;
    [SerializeField] private float angle;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float TurnSmoothVelocity;

    [Header("Object references")]
    [SerializeField] Transform Camera;
    [SerializeField] private CharacterController controller;
    [SerializeField] public Image StaminaBar;


    private void Awake()
    {
        playerControls = GetComponent<PlayerControls>();
    }


    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        sprint = playerControls.Player.Sprint;
        sprint.Enable();

        space = playerControls.Player.RollandBackstepandJump;
        space.Enable();


        playerControls.Player.Sprint.started += i => Shift_pressed = true;
        playerControls.Player.RollandBackstepandJump.started += i => Space_pressed = true;

    }

    private void OnDisable()
    {
        move.Disable();
        sprint.Disable();
        space.Disable();
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
        isGrounded = Grounded();

        if (isGrounded && gravityDirection.y < 0)
        {
            moveDirection.y = -2f;
        }

        // Get user input
        Vector2 Direction = move.ReadValue<Vector2>();
        moveDirection.x = 0f;
        moveDirection.z = 0f;

        // Movement
        if (Direction.magnitude >= 0.1f)
        {
            if (isGrounded)
            {
                targetAngle = Mathf.Atan2(Direction.x, Direction.y) * Mathf.Rad2Deg + Camera.eulerAngles.y;
                angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }
            else
            {
                moveDirection = transform.forward;
            }
        }

        // Find the moveSpeed
        MovementSpeedChange(Shift_pressed);

        // Move
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Jump calculation
        if (Space_pressed && Shift_pressed && isGrounded && StaminaBar.fillAmount > 0)
        {
            Space_pressed = false;
            gravityDirection.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        // Apply gravity
        gravityDirection.y += gravity * Time.deltaTime;

        // Jump
        controller.Move(gravityDirection * Time.deltaTime);

        // Roll
        if (!Shift_pressed && Space_pressed && Direction.magnitude >= 0.1f)
        {
            StartCoroutine(Roll());
        }

        // Reset buttons
        rollInvincibility = false;
        Space_pressed = false;
        Shift_pressed = false;
    }


    // Roll function
    IEnumerator Roll() 
    { 
        float startTime = Time.time;

        while (Time.time < startTime + rollTime)
        {
            rollInvincibility = true;
            controller.Move(moveDirection * rollSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Walk or run function
    private void MovementSpeedChange(bool Sprint)
    {
        if (Sprint && StaminaBar.fillAmount > 0)
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
