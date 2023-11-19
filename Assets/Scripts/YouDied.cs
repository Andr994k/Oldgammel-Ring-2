using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouDied : MonoBehaviour
{
    PlayerMechanics playerMechanics;

    [SerializeField] private GameObject player;

    private float currentHealthcheck;

    public Animator animator;

    private void Awake()
    {
        playerMechanics = player.GetComponent<PlayerMechanics>();
    }

    private void Update()
    {
        currentHealthcheck = playerMechanics.currentHealth;

        if (currentHealthcheck <= 0)
        {
            FadeToLevel();
        }
    }
    

    public void FadeToLevel()
    {
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(1);
    }

}
