using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperEnemy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerPosition;
    // [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterController characterController;

    // Declare movement variables
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float MaxDistance = 10;
    [SerializeField] private float MinDistance = 5;

    // Declare combat variables
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, playerPosition.position) <= MaxDistance)
        {
            ChasePlayer();
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }


    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt animation
        animator.SetTrigger("Hurt");

        // Check if enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");

        // Play die animation
        animator.SetBool("IsDead", true);

        // Disable the enemy
        GetComponent<CharacterController>().enabled = false;
        this.enabled = false;
    }

    void ChasePlayer()
    {
        transform.LookAt(playerPosition);

        if (Vector3.Distance(transform.position, playerPosition.position) >= MinDistance)
        {
            characterController.Move(transform.forward * moveSpeed * Time.deltaTime);
            animator.SetBool("IsWalking", true);
        }

        else
        {
            animator.SetBool("IsWalking", false);
        }

    }

}
