using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject ui;
    [SerializeField] private float distance = 1f;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool healthDrop;
    [SerializeField] private GameObject health;
    [SerializeField] private bool damageDrop;

    [SerializeField] private AudioSource ChestAudio;

    bool isChestOpen = false;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, playerPosition.position) <= distance && isChestOpen != true)
        {
            ui.SetActive(true);
            playerInput.actions["Interact"].performed += ctx => ChestInteract();
        }
        else
        {
            ui.SetActive(false);
        }
    }

    void ChestInteract()
    {
        animator.SetBool("IsOpen", true);
        ChestAudio.Play();
        isChestOpen = true;
        DropItem();

        if (animator == null)
        {
            return;
        }

        if (ChestAudio == null)
        {
            return;
        }
        
    }

    void DropItem()
    {
        if (healthDrop == true)
        {
            Debug.Log("spawned item");
            GameObject healthClone;
            healthClone = Instantiate(health, spawnPoint.position, Quaternion.identity);
            healthClone.transform.position = spawnPoint.position;
            ui.SetActive(false);
            Destroy(this.gameObject);
        }
    }
}
