using System;
using UnityEngine;

public class GhostCollider : MonoBehaviour
{
    public bool isPlayerInRange = false;
    public bPlayerController playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = other.gameObject.GetComponent<bPlayerController>();
            isPlayerInRange = true;    
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = null;
            isPlayerInRange = false;
        }
            
    }
}
