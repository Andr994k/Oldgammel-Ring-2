using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerMechanics : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float currentHealth;
    [SerializeField] public Image HealthBar;
    [SerializeField] private int flaskOfCrimsonTears;
    [SerializeField] private TextMeshProUGUI flaskOfCrimsonTearsAmount;
    [SerializeField] private float flaskOfCrimsonTearsHealAmount;
    [SerializeField] private bool invincible;

    [Header("Mana")]
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina;
    [SerializeField] private float StaminaRunCost;
    [SerializeField] private float StaminaJumpCost;
    [SerializeField] private float StaminaRollCost;
    [SerializeField] private float StaminaRechargeRate;

    [SerializeField] public Image StaminaBar;

    [Header("Inputs")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private InputAction roll;
    [SerializeField] private InputAction sprint;
    [SerializeField] private InputAction tab;
    [SerializeField] private InputAction E;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] PlayerMovement playerMovement;

    [SerializeField] private bool E_pressed;
    [SerializeField] private bool Space_pressed;


    private void Awake()
    {
        playerControls = new PlayerControls();
        controlsScreen.SetActive(false);
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        roll = playerControls.Player.RollandBackstepandJump;
        roll.Enable();

        sprint = playerControls.Player.Sprint;
        sprint.Enable();

        tab = playerControls.Player.Controls;
        tab.Enable();

        E = playerControls.Player.Healing;
        E.Enable();

        playerControls.Player.Healing.started += i => E_pressed = true;
        playerControls.Player.RollandBackstepandJump.started += i => Space_pressed = true;
    }

    private void Start()
    {
        currentHealth = maxHealth; currentMana = maxMana; currentStamina = maxStamina;
        StaminaBar.fillAmount = maxStamina/100;
        flaskOfCrimsonTears = 3;
        flaskOfCrimsonTearsAmount.text = $"{flaskOfCrimsonTears}";
    }

    private void Update()
    {
        invincible = playerMovement.rollInvincibility;

        bool isGrounded = Grounded();

        float Sprinting = sprint.ReadValue<float>();
        float showControls = tab.ReadValue<float>();

        // Stamina control
        if (Sprinting > 0 && isGrounded) // && Jumping == 0
        {
            StaminaBar.fillAmount -= StaminaRunCost * Time.deltaTime;
            currentStamina -= StaminaRunCost * 100 * Time.deltaTime;
            if (Space_pressed && StaminaBar.fillAmount > 0.1f)
            {
                Space_pressed = false;
                StaminaBar.fillAmount -= StaminaJumpCost/100;
                currentStamina -= StaminaJumpCost;
            }
            //Space_pressed = false;
        }
        else if (isGrounded && !Space_pressed)
        {
            StaminaBar.fillAmount += StaminaRechargeRate * Time.deltaTime;
            currentStamina += StaminaRechargeRate * 100 * Time.deltaTime;
        }
        
        if (isGrounded && invincible)
        {
            Space_pressed = false;
            StaminaBar.fillAmount -= StaminaRollCost / 100;
            currentStamina -= StaminaRollCost;
        }
        Space_pressed = false;

        if (StaminaBar.fillAmount < 0 | currentStamina < 0)
        {
            StaminaBar.fillAmount = 0;
            currentStamina = 0;
        }
        if (StaminaBar.fillAmount > maxStamina | currentStamina > maxStamina)
        {
            StaminaBar.fillAmount = maxStamina;
            currentStamina = maxStamina;
        }


        // Controls screen
        if (showControls > 0)
        {
            controlsScreen.SetActive(true);
        }
        else
        {
            controlsScreen.SetActive(false);
        }


        // Heal
        if (E_pressed && flaskOfCrimsonTears > 0)
        {
            E_pressed = false;
            flaskOfCrimsonTears -= 1;
            flaskOfCrimsonTearsAmount.text = $"{flaskOfCrimsonTears}";
            currentHealth += flaskOfCrimsonTearsHealAmount;
            HealthBar.fillAmount += flaskOfCrimsonTearsHealAmount/100;
        }
        E_pressed = false;

        if (currentHealth > maxHealth) 
        { 
            currentHealth = maxHealth;
            HealthBar.fillAmount = maxHealth;
        }
    }

    private bool Grounded()
    {
        return Physics.Raycast(
            transform.position + controller.center, Vector3.down,
        controller.bounds.extents.y + controller.skinWidth + 0.05f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Weapon" && !invincible)
        {
            currentHealth -= 0.5f * 100 * Time.deltaTime;
            HealthBar.fillAmount -= 0.5f * Time.deltaTime;
        }
    }

}
