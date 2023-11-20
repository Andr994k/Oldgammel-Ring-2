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
    [SerializeField] private InputAction jump;
    [SerializeField] private InputAction space;
    [SerializeField] private bool Shift_pressed;
    [SerializeField] private bool Space_pressed;
    [SerializeField] public bool F_pressed;

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float rollTime;
    [SerializeField] private float backstepSpeed = 5f;
    [SerializeField] public bool isGrounded;
    [SerializeField] public float Sprinting;
    [SerializeField] public Vector2 Direction;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    [SerializeField] public bool rollInvincibility;

    [Header("Gravity")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = -9.82f;
    [SerializeField] private Vector3 gravityDirection = Vector3.zero;

    [Header("Angle")]
    [SerializeField] private float targetAngle;
    [SerializeField] private float angle;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float TurnSmoothVelocity;

    [Header("Object references")]
    [SerializeField] private Transform Camera;
    [SerializeField] private CharacterController controller;
    [SerializeField] public Image StaminaBar;
    [SerializeField] private GameObject me;




    private void Awake()
    {
        playerControls = new PlayerControls();
    }


    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        sprint = playerControls.Player.Sprint;
        sprint.Enable();

        space = playerControls.Player.RollandBackstep;
        space.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();

        playerControls.Player.Sprint.started += i => Shift_pressed = true;
        playerControls.Player.RollandBackstep.started += i => Space_pressed = true;
        playerControls.Player.Jump.started += i => F_pressed = true;

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
        moveSpeed = walkSpeed;
        //rollInvincibility = false;
        // Get user input
        Sprinting = sprint.ReadValue<float>();
        Direction = move.ReadValue<Vector2>();
        moveDirection.x = 0f;
        moveDirection.z = 0f;

        // Movement
        if (Direction.magnitude > 0f)
        {
            //if (isGrounded)
            //{
                targetAngle = Mathf.Atan2(Direction.x, Direction.y) * Mathf.Rad2Deg + Camera.eulerAngles.y;
                angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            //}
            //else
            //{
            //    moveDirection = transform.forward;
            //}
        }


        // Do we Sprint or walk
        if (Sprinting == 1 && StaminaBar.fillAmount > 0 && !Space_pressed)
        {
            Sprint();
        }

        // Forward/back and left/right movement
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Do we jump
        if (F_pressed && isGrounded && StaminaBar.fillAmount > 0f)
        {
            Jump();
        }

        // Apply gravity
        Gravity();

        // Up and Down Movement
        controller.Move(gravityDirection * Time.deltaTime);

        // Roll
        if (Space_pressed && isGrounded && StaminaBar.fillAmount > 0.05f)
        {
            StartCoroutine(Roll());

        }

        // Reset buttons
        Space_pressed = false;
        F_pressed = false;
    }


    // Walk or run function
    private void Sprint()
    {
        moveSpeed = runSpeed;
        Shift_pressed = false;
    }

    private void Jump()
    {
        F_pressed = false;
        gravityDirection.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    private void Gravity()
    {
        gravityDirection.y += gravity * Time.deltaTime;

        // Constrain gravity
        if (isGrounded && gravityDirection.y < 0)
        {
            gravityDirection.y = -2f;
        }
    }

    // Roll function
    IEnumerator Roll() 
    { 
        float startTime = Time.time;

        while (Time.time < startTime + rollTime)
        {
            if (Direction.magnitude > 0.1f)
            {
                controller.Move(moveDirection * rollSpeed * Time.deltaTime);
            }
            else
            {
                controller.Move(me.transform.forward * -1 * rollSpeed * Time.deltaTime);
            }
            rollInvincibility = true;
            yield return null;
        }
        rollInvincibility = false;
    }


    // Ground check
    private bool Grounded()
    {
        return Physics.Raycast(
            transform.position + controller.center, Vector3.down,
            controller.bounds.extents.y + controller.skinWidth + 0.01f);
    }
}
