using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimations : MonoBehaviour
{

    [SerializeField] private AI ai;
    [SerializeField] private bool moving;
    public Animator animator;


    private void Awake()
    {
        ai = gameObject.GetComponent<AI>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        moving = ai.Moving;

        if (moving) 
        {
            animator.SetFloat("Moving", 1f, 0.1f, Time.deltaTime);
            animator.SetFloat("Attacking", 0f, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Moving", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Attacking", 1f, 0.1f, Time.deltaTime);
        }



    }




}

