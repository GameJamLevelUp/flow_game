using UnityEngine;

public class BadBoat : MonoBehaviour
{
    public Transform player; // Reference to the player's transform

    private Rigidbody2D rb2D; // Reference to the Rigidbody2D component
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Try to get the Rigidbody2D component for 2D physics
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned to BadBoat script.");
            return;
        }

        // Calculate the direction to the player
        Vector3 direction = (player.position - transform.position).normalized;

        // Rotate towards the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (rb2D != null)
        {
            // For 2D physics
            rb2D.rotation = angle;
            rb2D.velocity = direction * 3f;
        }
        else
        {
            // Fallback in case no rigidbody is attached
            transform.position += direction * 1f * Time.deltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
