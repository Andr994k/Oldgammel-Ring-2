using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public Camera cameraObject;
    public PlayerManager player;

    [Header("Camera Settings")]
    private Vector3 cameraVelocity;
    private float CameraSmoothSpeed = 1;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            FollowPlayer();
        }

    }

    private void FollowPlayer()
    {
        Vector3 tagetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, CameraSmoothSpeed * Time.deltaTime);
        transform.position = tagetCameraPosition;

    }




}
