using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float playerSpeedThreshold = 20f; // Speed threshold below which the player takes damage
    public int damageAmount = 10; // Amount of damage to inflict on the player
    public Boolean shouldDestroy = true;
    public ParticleSystem explosionParticleSystem;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with a player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the player's Rigidbody2D component to check speed
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Check player's speed
                float playerSpeed = playerRb.velocity.magnitude;

                // If player speed is below the threshold, apply damage
                if (playerSpeed < playerSpeedThreshold)
                {
                    // Find the GameUI instance and apply damage
                    GameUI gameUI = GameObject.FindObjectOfType<GameUI>();
                    if (gameUI != null)
                    {
                        gameUI.ReceiveDamage();
                    }
                } else if (shouldDestroy && explosionParticleSystem != null) {
                        if (shouldDestroy && explosionParticleSystem != null)
                        {
                            explosionParticleSystem.Play();
                            Destroy(gameObject, 0.2f);
                        }
                }
            }

            
            
        }
    }
}
