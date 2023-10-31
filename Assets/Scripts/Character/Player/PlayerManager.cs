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

        playerMovementManager.HandleAllMovement();
    }
}
