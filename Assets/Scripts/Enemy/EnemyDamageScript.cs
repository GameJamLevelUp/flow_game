using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float playerSpeedThreshold = 20f; // Speed threshold below which the player takes damage
    public int damageAmount = 10; // Amount of damage to inflict on the player
    public Boolean shouldDestroy = true;
    public ParticleSystem explosionParticleSystem;
    private bool shouldDetect = false;

    public float rewardForKill = 0f;
    public GameObject rewardPrefab;
   private void Start()
    {
        StartCoroutine(InitializeDetection());
    }

    private IEnumerator InitializeDetection()
    {
        yield return new WaitForSeconds(0.1f);
        shouldDetect = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with a player
        if (collision.gameObject.CompareTag("Player") && shouldDetect)
        {
            // Get the player's Rigidbody2D component to check speed
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Check player's speed
                float playerSpeed = playerRb.velocity.magnitude;

                if (playerSpeed < playerSpeedThreshold)
                {
                    // Find the GameUI instance and apply damage
                    GameUI gameUI = GameObject.FindObjectOfType<GameUI>();
                    if (gameUI != null)
                    {
                        gameUI.ReceiveDamage();
                    }
                } else if (shouldDestroy && explosionParticleSystem != null)
                {
                    explosionParticleSystem.Play();
                    Destroy(gameObject, 0.2f);           
                }
                else if (shouldDestroy)
                {
                    GameUI gameUI = GameObject.FindObjectOfType<GameUI>();

                    if (playerSpeedThreshold == 0)
                    {
                        if (gameUI != null)
                        {
                            gameUI.ReceiveDamage();
                        }
                    }
                    if (rewardForKill != 0f)
                    {
                        gameUI.AddAdditionalDistance(rewardForKill);
                        GameObject text = Instantiate(rewardPrefab, transform.position, Quaternion.identity);
                        text.GetComponentInChildren<TextMeshProUGUI>().text = $"+ {rewardForKill}m";
                    }


                    Destroy(gameObject);           
                }
            }

            
            
        }
    }
}
