using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{

    public CharacterController characterController;

    CharacterNetworkManager characterNetworkManager;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
    }
    
    protected virtual void Update()
    {
        // If its controlled from our side, then assign its network position to the transform position
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        // If its controlled from another client, then assign its position to the network position
        else
        {
            // Position
            transform.position = Vector3.SmoothDamp
                (transform.position, 
                characterNetworkManager.networkPosition.Value, 
                ref characterNetworkManager.networkPositionVelocity, 
                characterNetworkManager.networkPositionSmoothTime);

            // Rotation
            transform.rotation = Quaternion.Slerp
                (transform.rotation, 
                characterNetworkManager.networkRotation.Value, 
                characterNetworkManager.networkRotationSmoothTime);
        }
    }
}
