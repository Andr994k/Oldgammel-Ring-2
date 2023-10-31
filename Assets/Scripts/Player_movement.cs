using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = 9.81f;

    private CharacterController controller;
    private bool isGrounded = false;

    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if we're on the ground
        isGrounded = GroundControl();

        // Get user input (old input system)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool wantJump = Input.GetButtonDown("Jump");

        // Reset y velocity when we hit the ground
        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = 0;
        }

        // Handle movement on the ground
        if (isGrounded)
        {
            moveDirection = new Vector3(h, moveDirection.y, v).normalized * moveSpeed;

            // Face in the move direction
            if (h != 0 || v != 0)
            {
                transform.forward = new Vector3(h, 0f, v);
            }
        }

        // Handle jumping
        if (isGrounded && wantJump)
        {
            moveDirection.y = Mathf.Sqrt(2f * gravity * jumpHeight);
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move
        controller.Move(moveDirection * Time.deltaTime);
    }

    // Built-in ground check is bad, so use raycast instead
    private bool GroundControl()
    {
        return Physics.Raycast(
            transform.position + controller.center,                     // from the middle of the controller...
            Vector3.down,                                               // ...pointing downwards...
            controller.bounds.extents.y + controller.skinWidth + 0.2f); // ... to the bottom of the controller.
    }
}
