using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;

    PlayerControls playerControls;

    [SerializeField] Vector2 movementInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // When the scene changes
        SceneManager.activeSceneChanged += OnSceneChange;

        Instance.enabled = false;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        // if we are loading into the world scene then enable controls.
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            Instance.enabled = true;
        }
        // if not we are at the main menu.
        else
        {
            Instance.enabled = false;
        }
    }


    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }


    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }
}
