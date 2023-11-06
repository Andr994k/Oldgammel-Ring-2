using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerMovementManager playerMovementManager;
    protected override void Awake()
    {
        base.Awake();

        playerMovementManager = GetComponent<PlayerMovementManager>();
    }

    protected override void Update()
    {
        base.Update();

        // If you dont own the gameobject you cant control or edit it
        if (!IsOwner)
            return;

        playerMovementManager.HandleAllMovement();
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
        }
    }

}
