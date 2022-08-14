using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadierEnemy : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject winScreen;

    // Declare movement variables
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float MaxDistance = 12;
    [SerializeField] private float MinDistance = 3;
    [SerializeField] private float MinDistanceForMelee = 2;

    // Declare combat variables
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform attackPoint2;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] Rigidbody projectile;

    [SerializeField] private int maxHealth = 500;
    [SerializeField] private int currentHealth;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackRange2 = 5f;
    [SerializeField] private int attackDamage = 60;
    [SerializeField] private float attackCooldown = 10f;
    [SerializeField] private float attackCooldownRange = 8f;
    [SerializeField] private float grenadeVelocity = 6f;

    // Declare Audio variables
    [SerializeField] private AudioSource deathAudio;

    float nextAttackTime;
    float nextAttackTimeRanged;

    bool isDead = false;
    bool isEnemyAttacking = false;
    int cycleAttacks = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(maxHealth);
    }

    // Update is called once per frame
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

        if (Vector3.Distance(transform.position, playerPosition.position) <= MinDistance && isEnemyAttacking != true)
        {
            transform.LookAt(playerPosition);
        }

        if (Vector3.Distance(transform.position, playerPosition.position) <= MinDistanceForMelee && nextAttackTime <= 0)
        {
            nextAttackTime = attackCooldown;
            AttackPlayer();
        }

        if (Vector3.Distance(transform.position, playerPosition.position) >= MinDistanceForMelee && Vector3.Distance(transform.position, playerPosition.position) <= MaxDistance && nextAttackTimeRanged <= 0)
        {
            nextAttackTimeRanged = attackCooldownRange;
            AttackPlayerAtRange();
        }

        if (nextAttackTimeRanged > 0)
        {
            nextAttackTimeRanged -= Time.deltaTime;
        }

        if (nextAttackTime > 0)
        {
            nextAttackTime -= Time.deltaTime;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("GrenadierMeleeAttack") || animator.GetCurrentAnimatorStateInfo(0).IsName("GrenadierCloseRangeAttack") || animator.GetCurrentAnimatorStateInfo(0).IsName("GrenadierRangeAttack"))
        {
            isEnemyAttacking = true;
        }
        else
        {
            isEnemyAttacking = false;
        }
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
        if (cycleAttacks == 0)
        {
            animator.SetTrigger("Attack");
            
        }

        if (cycleAttacks == 1)
        {
            animator.SetTrigger("Attack2");

        }

    }

    void AttackPlayerAtRange()
    {
        animator.SetTrigger("Attack3");
    }

    void StartAttack()
    {
        //Detect enemies in range of attack
        Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers);

        cycleAttacks = 1;

        foreach (Collider player in hitPlayer)
        {
            Debug.Log("Enemy hit " + player.name);

            if (player.GetComponent<PlayerController>() != null)
            {
                player.GetComponent<PlayerController>().PlayerTakeDamage(attackDamage);
            }
        }
    }

    void EndAttack()
    {
        return;
    }

    void StartAttack2()
    {
        //Detect enemies in range of attack
        Collider[] hitPlayer = Physics.OverlapSphere(attackPoint2.position, attackRange2, playerLayers);

        cycleAttacks = 0;

        foreach (Collider player in hitPlayer)
        {
            Debug.Log("Enemy hit " + player.name);

            if (player.GetComponent<PlayerController>() != null)
            {
                player.GetComponent<PlayerController>().PlayerTakeDamage(attackDamage);
            }
        }
    }

    void EndAttack2()
    {
        return;
    }

    void ActivateShield()
    {
        // Instantiate the projectile at the position and rotation of this transform
        Rigidbody clone;
        Rigidbody clone2;
        Rigidbody clone3;
        Rigidbody clone4;
        clone = Instantiate(projectile, spawnPoint.position, transform.rotation);
        clone2 = Instantiate(projectile, spawnPoint.position + new Vector3(0, 1, 0), transform.rotation);
        clone3 = Instantiate(projectile, spawnPoint.position + new Vector3(0, 2, 0), transform.rotation);
        clone4 = Instantiate(projectile, spawnPoint.position + new Vector3(0, 3, 0), transform.rotation);

        // Give the cloned object an initial velocity along the current
        // object's Z axis
        clone.velocity = transform.TransformDirection(Vector3.forward * grenadeVelocity);
        clone2.velocity = transform.TransformDirection(Vector3.back * grenadeVelocity);
        clone3.velocity = transform.TransformDirection(Vector3.left * grenadeVelocity);
        clone4.velocity = transform.TransformDirection(Vector3.right * grenadeVelocity);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(attackPoint2.position, attackRange2);
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
        this.enabled = false;
        winScreen.SetActive(true);
        deathAudio.Play();
    }
}
