using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
    [SerializeField] private Transform damagePoint;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float damageRange = 0.5f;
    [SerializeField] private int trapDamage = 10;
    [SerializeField] private int enemyDamageMultiplier = 3;
    [SerializeField] private float trapCooldown = 5f;
    [SerializeField] private float trapTimeBeforeActive = 1f;

    float trapResetTime = 0;

    void Update()
    {

        if (trapResetTime > 0)
        {
            trapResetTime -= Time.deltaTime;
        }
        else
        {
            animator.SetBool("IsActivated", false);
        }

        Collider[] checkForEnemy = Physics.OverlapSphere(damagePoint.position, damageRange, enemyLayers);

        if (trapResetTime <= 0 && checkForEnemy.Length > 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Trap Close"))
        {
            trapResetTime = trapCooldown;
            for (int i = 0; i < checkForEnemy.Length; i++)
            {
                checkForEnemy[i] = null;
            }
            Debug.Log("Player in trap!");
            StartCoroutine(WaitUntilActive());
        }
    }

    void DamageEnemy()
    {
        trapResetTime = trapCooldown;

        // play animation
        animator.SetBool("IsActivated", true);

        //Detect enemies in range of attack
        Collider[] hitPlayer = Physics.OverlapSphere(damagePoint.position, damageRange, enemyLayers);

        //Damage the enemies
        foreach (Collider enemy in hitPlayer)
        {
            Debug.Log("Trap hit " + enemy.name);

            if (enemy.GetComponent<PlayerController>() != null)
            {
                enemy.GetComponent<PlayerController>().PlayerTakeDamage(trapDamage);
            }
            if (enemy.GetComponent<ChomperEnemy>() != null)
            {
                enemy.GetComponent<ChomperEnemy>().TakeDamage(trapDamage * enemyDamageMultiplier);
            }
        }
    }

    IEnumerator WaitUntilActive()
    {
        yield return new WaitForSeconds(trapTimeBeforeActive);
        Debug.Log("Trap activated!");
        DamageEnemy();
    }

    void OnDrawGizmosSelected()
    {
        if (damagePoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(damagePoint.position, damageRange);
    }
}
