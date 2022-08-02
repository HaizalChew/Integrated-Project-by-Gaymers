using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperEnemy : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;


    void Start()
    {
        currentHealth = maxHealth;
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
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }
}
