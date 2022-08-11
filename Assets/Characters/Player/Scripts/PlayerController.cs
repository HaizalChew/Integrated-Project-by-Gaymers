using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Declare all variables
    // [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private HealthBar healthBar;

    // Declare movement variable
    [SerializeField] private float speed = 10;
    [SerializeField] private float turnSmoothTime = 0.1f;

    // Declare dash variable
    [SerializeField] private float dashSpeedMultiplier = 2;
    [SerializeField] private bool isPlayerDash = false;
    [SerializeField] private float dashTimer = 1f;
    float dashPlaceholder;

    // Declare attack variables
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 30;
    [SerializeField] private float attackCooldown = 2f;
    float nextAttackTime = 0f;
    int noOfClicks = 0;
    float lastClickedTime = 0;
    float maxComboDelay = 1.1f;

    // Declare health variables
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    Vector2 input;
    Vector3 movementInput;
    
    

    Animator mAnimator;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
        dashPlaceholder = dashTimer;

        currentHealth = maxHealth;
        healthBar.SetHealth(maxHealth);
    }

    void Update()
    {
        if (nextAttackTime <= 0)
        {
            Move();
        }
        
        Look();

        //If player is moving, play run animaton.
        if (movementInput != Vector3.zero)
        {
            mAnimator.SetBool("isWalk", true);
        }
        else
        {
            mAnimator.SetBool("isWalk", false);
        }

        //Player dash controls
        playerInput.actions["Dash"].performed += ctx => playerDashed();

        if (isPlayerDash == true)
        {
            dashTimer -= Time.deltaTime;
        }
        
        if (dashTimer <= 0.0f)
        {
            isPlayerDash = false;
            dashTimer = dashPlaceholder;
        }

        //Player Attack Controls
        playerInput.actions["Attack"].performed += ctx => Attack();

        if (nextAttackTime > 0)
        {
            nextAttackTime -= Time.deltaTime;
        }

        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }        
    }


    //Controlled by Input System
    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
        movementInput = new Vector3(input.x, 0, input.y);
    }

    void playerDashed()
    {
        if (dashTimer == dashPlaceholder)
        {
            isPlayerDash = true;
        }
        
    }

    //Functions
    void Move()
    {
        if (isPlayerDash == true && dashTimer >= 0.0f)
        {
            // rb.MovePosition(transform.position + (transform.forward * movementInput.magnitude) * speed * Time.deltaTime * dashSpeedMultiplier);
            characterController.Move((transform.forward * movementInput.magnitude) * speed * Time.deltaTime * dashSpeedMultiplier);
        }
        else
        {
            // rb.MovePosition(transform.position + (transform.forward * movementInput.magnitude) * speed * Time.deltaTime);
            characterController.Move((transform.forward * movementInput.magnitude) * speed * Time.deltaTime);
        }
        
    }

    void Look()
    {
        if (movementInput != Vector3.zero)
        {
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45 , 0));

            var skewedInput = matrix.MultiplyPoint3x4(movementInput);

            var relative = (transform.position + skewedInput) - transform.position;
            var rotation = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSmoothTime);
        }
        
    }

    void Attack()
    {
        if (nextAttackTime <= 0)
        {
            nextAttackTime = attackCooldown;
            lastClickedTime = Time.time;
            noOfClicks++;
            noOfClicks = Mathf.Clamp(noOfClicks, 0, 4);

            if (noOfClicks == 1)
            {
                //Play an Attack Animation
                mAnimator.SetTrigger("isAttack");
            }

            if (noOfClicks == 2)
            {
                //Play an Attack Animation
                mAnimator.SetTrigger("isAttack2");
                nextAttackTime += 0.2f;
            }

            if (noOfClicks == 3)
            {
                //Play an Attack Animation
                mAnimator.SetTrigger("isAttack3");
                nextAttackTime += 0.4f;
            }

            if (noOfClicks == 4)
            {
                //Play an Attack Animation
                mAnimator.SetTrigger("isAttack4");
                noOfClicks = 0;
                nextAttackTime += 0.6f;
            }

        }

    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void PlayerTakeDamage(int damage)
    {
        currentHealth -= damage;
        mAnimator.SetTrigger("Hurt");
        nextAttackTime += 0.5f;

        // set the health bar
        healthBar.SetHealth(currentHealth);

        // Check if player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");

        // Play die animation
        mAnimator.SetBool("isDead", true);

        // Disable the player
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerInput>().enabled = false;
        this.enabled = false;
    }

    void MeleeAttackStart()
    {
        //Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        //Damage the enemies
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);

            if (enemy.GetComponent<ChomperEnemy>() != null)
            {
                enemy.GetComponent<ChomperEnemy>().TakeDamage(attackDamage);
            }

            if (enemy.GetComponent<GrenadierEnemy>() != null)
            {
                enemy.GetComponent<GrenadierEnemy>().TakeDamage(attackDamage);
            }

        }
    }

    void MeleeAttackEnd()
    {
        return;
    }
}
    