using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperEnemy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerPosition;
    // [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private HealthBar healthBar;


    // Declare movement variables
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float MaxDistance = 10;
    [SerializeField] private float MinDistance = 5;

    // Declare combat variables
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 30;
    [SerializeField] private float attackCooldown = 2f;

    float nextAttackTime;
    bool isEnemyAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(maxHealth);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, playerPosition.position) <= MaxDistance)
        {
            ChasePlayer();
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        if (Vector3.Distance(transform.position, playerPosition.position) <= MinDistance && nextAttackTime <= 0)
        {
            nextAttackTime = attackCooldown;
            AttackPlayer();
        }

        if (nextAttackTime > 0)
        {
            nextAttackTime -= Time.deltaTime;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ChomperAttack"))
        {
            isEnemyAttacking = true;
        }
        else
        {
            isEnemyAttacking = false;
        }

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt animation
        animator.SetTrigger("Hurt");

        // set the health bar
        healthBar.SetHealth(currentHealth);

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
       
        if (Vector3.Distance(transform.position, playerPosition.position) >= MinDistance && isEnemyAttacking != true)
        {
            transform.LookAt(playerPosition);
            characterController.Move(transform.forward * moveSpeed * Time.deltaTime);
            animator.SetBool("IsWalking", true);
        }

        else
        {
            animator.SetBool("IsWalking", false);
        }

    }

    void AttackPlayer()
    {
        animator.SetTrigger("Attack");
    }

    void AttackBegin()
    {
        //Detect enemies in range of attack
        Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers);

        foreach (Collider player in hitPlayer)
        {
            Debug.Log("Enemy hit " + player.name);

            player.GetComponent<PlayerController>().PlayerTakeDamage(attackDamage);
        }
    }

    void AttackEnd()
    {
        return;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
