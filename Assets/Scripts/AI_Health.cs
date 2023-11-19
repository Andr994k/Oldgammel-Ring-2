using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AI_Health : MonoBehaviour
{
    [SerializeField] private GameObject player;


    private void Update()
    {
        LookAtTarget();
    }

    void LookAtTarget()
    {
        Vector3 lookAt = player.transform.position;
        lookAt.y = transform.position.y;

        Vector3 lookDir = (lookAt - transform.position).normalized;
        transform.forward = lookDir;
    }

}
