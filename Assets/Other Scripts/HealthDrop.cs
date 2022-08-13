using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : MonoBehaviour
{

    [SerializeField] private float range = 1f;
    [SerializeField] private int health = 30;
    [SerializeField] private LayerMask playerLayers;


    // Update is called once per frame
    void Update()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(transform.position, range, playerLayers);

        if (hitPlayer.Length > 0)
        {
            Debug.Log("Healed players!");
            foreach (Collider player in hitPlayer)
            {
                player.GetComponent<PlayerController>().PlayerTakeDamage(-health);
            }

            Destroy(this.gameObject);
        }
        
    }

    void OnDrawGizmosSelected()
    {
        if (transform.position == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(transform.position, range);
    }

}
