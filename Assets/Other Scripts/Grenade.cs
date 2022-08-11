using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private Material material;
    [SerializeField] private float timeToExplode = 8f;
    [SerializeField] private float pulseValue = 0;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int attackDamage = 30;

    public float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_Pulse_Value", pulseValue);

        timer += Time.deltaTime;

        pulseValue = 0.5f * Mathf.Sin(Mathf.Pow(2f, timer)) + 0.5f;

        if (timer >= timeToExplode)
        {
            DoDamage();
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void DoDamage()
    {
        //Detect enemies in range of attack
        Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        //Damage the enemies
        foreach (Collider enemy in hitPlayer)
        {
            Debug.Log("Trap hit " + enemy.name);

            if (enemy.GetComponent<PlayerController>() != null)
            {
                enemy.GetComponent<PlayerController>().PlayerTakeDamage(attackDamage);
            }
            if (enemy.GetComponent<ChomperEnemy>() != null)
            {
                enemy.GetComponent<ChomperEnemy>().TakeDamage(attackDamage);
            }
        }
    }
}
