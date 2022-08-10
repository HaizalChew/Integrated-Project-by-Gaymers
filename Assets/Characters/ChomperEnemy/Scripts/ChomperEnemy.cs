using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperEnemy : MonoBehaviour
{
    // Declare ALL variables
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerPosition;
    // [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject canvas;

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

    // Declare Shader Materials
    [SerializeField] private Material material1;
    [SerializeField] private GameObject bodyObj;
    float dissolveValue = 0;
    bool isDead = false;

    float nextAttackTime;
    bool isEnemyAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(maxHealth);
        
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, playerPosition.position) <= MaxDistance && isDead != true)
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

        if (isDead == true)
        {
            DieDissolve();
            dissolveValue += Time.deltaTime / 2f;
        }
        if (dissolveValue >= 1)
        {
            Destroy(this.gameObject);
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
            isDead = true;
            Die();
        }
    }

    void Die()
    {

        // Play die animation
        animator.SetBool("IsDead", true);

        // Disable the enemy
        GetComponent<CharacterController>().enabled = false;
        canvas.SetActive(false);

    }

    void DieDissolve()
    {
        bodyObj.GetComponent<SkinnedMeshRenderer>().material = material1;
        material1.SetFloat("_Dissolve_Value", dissolveValue);
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
