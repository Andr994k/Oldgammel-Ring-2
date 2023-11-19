using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerMechanics playerMechanics;
    private Vector2 direction;
    private float sprinting;
    private float stamina;
    private bool grounded;
    private bool F_pressed;
    private bool isAttacking = false;
    public Animator animator;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerMechanics = GetComponent<PlayerMechanics>();
    }


    private void Update()
    {
        direction = playerMovement.Direction;
        sprinting = playerMovement.Sprinting;
        stamina = playerMechanics.currentStamina;
        grounded = playerMovement.isGrounded;
        F_pressed = playerMovement.F_pressed;


        if (direction.magnitude >= 0.1f && sprinting == 0f && grounded) 
        {
            animator.SetFloat("Idle", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Move", 0.6f, 0.1f, Time.deltaTime);
            animator.SetFloat("Jump", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Attack", 0f, 0.1f, Time.deltaTime);
        }

        if (direction.magnitude >= 0.1f && sprinting == 1f && stamina > 0f && grounded)
        {
            animator.SetFloat("Idle", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Move", 1f, 0.1f, Time.deltaTime);
            animator.SetFloat("Jump", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Attack", 0f, 0.1f, Time.deltaTime);
        }
        if (!grounded)
        {
            animator.SetFloat("Idle", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Move", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Jump", 1f, 0.1f, Time.deltaTime);
            animator.SetFloat("Attack", 0f, 0.1f, Time.deltaTime);
        }
        if (Input.GetMouseButton(0) && grounded)
        {
            isAttacking = true;
            animator.SetFloat("Idle", 0f);
            animator.SetFloat("Move", 0f);
            animator.SetFloat("Jump", 0f);
            animator.SetFloat("Attack", 1f);
        }
        if (direction.magnitude == 0f)
        {
            animator.SetFloat("Idle", 1f, 0.1f, Time.deltaTime);
            animator.SetFloat("Move", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Jump", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Attack", 0f, 0.1f, Time.deltaTime);
        }
    }

}
