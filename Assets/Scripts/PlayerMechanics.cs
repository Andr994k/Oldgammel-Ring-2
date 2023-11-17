using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using TMPro;

public class PlayerMechanics : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float currentHealth;
    [SerializeField] public Image HealthBar;
    [SerializeField] private int flaskOfCrimsonTears;
    [SerializeField] private TextMeshProUGUI flaskOfCrimsonTearsAmount;
    [SerializeField] private float flaskOfCrimsonTearsHealAmount = 50f;

    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana;

    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina;
    [SerializeField] private float StaminaRunCost = 0.1f;
    [SerializeField] private float StaminaJumpCost = 10f;
    [SerializeField] public Image StaminaBar;

    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private InputAction jump;
    [SerializeField] private InputAction sprint;
    [SerializeField] private InputAction tab;
    [SerializeField] private InputAction E;
    [SerializeField] private GameObject controlsScreen;
    
    [SerializeField] private bool E_pressed;

    public PlayerMechanics instance;

    private void Awake()
    {
        playerControls = new PlayerControls();
        controlsScreen.SetActive(false);
        instance = GetComponent<PlayerMechanics>();
    }

    private void OnEnable()
    {
        jump = playerControls.Player.Jump;
        jump.Enable();

        sprint = playerControls.Player.Sprint;
        sprint.Enable();

        tab = playerControls.Player.Controls;
        tab.Enable();

        E = playerControls.Player.Healing;
        E.Enable();

        playerControls.Player.Healing.started += i => E_pressed = true;
    }

    private void Start()
    {
        currentHealth = maxHealth; currentMana = maxMana; currentStamina = maxStamina;
        StaminaBar.fillAmount = maxStamina;
        flaskOfCrimsonTears = 3;
        flaskOfCrimsonTearsAmount.text = $"{flaskOfCrimsonTears}";
    }

    private void Update()
    {
        bool isGrounded = Grounded();

        float Jumping = jump.ReadValue<float>();
        float Sprinting = sprint.ReadValue<float>();
        float showControls = tab.ReadValue<float>();

        // Stamina control
        if (Sprinting > 0 && isGrounded && Jumping == 0)
        {
            StaminaBar.fillAmount -= StaminaRunCost * Time.deltaTime;
            currentStamina -= StaminaRunCost * 100 * Time.deltaTime;
        }
        else if (Jumping > 0 && isGrounded && Sprinting > 0)
        {
            StaminaBar.fillAmount -= StaminaJumpCost * Time.deltaTime;
            currentStamina -= StaminaJumpCost * 100 * Time.deltaTime;
        }
        else
        {
            StaminaBar.fillAmount += StaminaRunCost * Time.deltaTime;
            currentStamina += StaminaRunCost * 100 * Time.deltaTime;
        }
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
        if (other.transform.tag == "Weapon")
        {
            currentHealth -= 0.1f * 100 * Time.deltaTime;
            HealthBar.fillAmount -= 0.1f * Time.deltaTime;
        }
    }

}
