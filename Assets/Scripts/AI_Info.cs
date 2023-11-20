using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AI_Info : MonoBehaviour
{
    [SerializeField] private GameObject Camera;


    private void Update()
    {
        transform.rotation = Quaternion.identity;
        LookAtTarget();
    }

    void LookAtTarget()
    {
        Vector3 lookAt = Camera.transform.position;
        lookAt.y = transform.position.y;

        Vector3 lookDir = (lookAt - transform.position).normalized;
        transform.forward = lookDir;
    }

}
