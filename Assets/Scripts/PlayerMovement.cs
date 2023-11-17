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
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 7.5f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rollSpeed = 7.5f;
    [SerializeField] private float backstepSpeed = 5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private InputAction move;
    [SerializeField] private InputAction jump;
    [SerializeField] private InputAction sprint;
    [SerializeField] Vector3 moveDirection = Vector3.zero;


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
        isGrounded = Grounded();

        if (isGrounded && gravityDirection.y < 0)
        {
            moveDirection.y = -2f;
        }

        // Get user input
        Vector2 Direction = move.ReadValue<Vector2>();
        float Jumping = jump.ReadValue<float>();
        float Sprinting = sprint.ReadValue<float>();


        moveDirection.x = 0f;
        moveDirection.z = 0f;

        // movement
        if (Direction.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(Direction.x, Direction.y) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        // find the moveSpeed
        MovementSpeedChange(Sprinting);

        // Move
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        //if (Jumping > 0 && isGrounded)
        if (Input.GetButtonDown("Jump") && Sprinting > 0 && isGrounded && StaminaBar.fillAmount > 0.1f)
        {
            gravityDirection.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        gravityDirection.y += gravity * Time.deltaTime;

        controller.Move(gravityDirection * Time.deltaTime);
    }

    private void MovementSpeedChange(float S)
    {
        if (S > 0 && StaminaBar.fillAmount > 0)
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
