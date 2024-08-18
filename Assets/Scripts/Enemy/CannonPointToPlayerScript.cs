using UnityEngine;

public class CannonPointToPlayerScript : MonoBehaviour
{
    private Transform player;  // Reference to the player's transform
    public float rotationSpeed = 5f;  // Speed at which the cannon rotates towards the player
    public float projectileSpeed = 5f;  // Speed at which the projectile travels
    public GameObject cannonballPrefab;  // Cannonball prefab to instantiate
    public float shootingDistance = 10f;  // Maximum distance to the player for shooting
    public float shootingInterval = 2.5f;  // Time between shots

    private Rigidbody2D playerRigidbody;
    private float timeSinceLastShot = 0f;

    void Start()
    {
        player = GameObject.FindObjectOfType<SwingingController>().transform;
        playerRigidbody = player.GetComponent<Rigidbody2D>();

        if (playerRigidbody == null)
        {
            Debug.LogWarning("Player Rigidbody2D not found.");
        }
    }

    void Update()
    {
        if (player == null || playerRigidbody == null)
        {
            Debug.LogWarning("Player Transform or Rigidbody2D not assigned.");
            return;
        }

        // Calculate the direction from the cannon to the player
        Vector3 directionToPlayer = player.position - transform.position;

        // Predict the player's future position based on their current velocity
        float timeToTarget = directionToPlayer.magnitude / projectileSpeed;
        Vector3 predictedPosition = (Vector3)playerRigidbody.position + (Vector3)playerRigidbody.velocity * timeToTarget;

        // Calculate the direction to the predicted position
        Vector3 directionToPredictedPosition = predictedPosition - transform.position;

        // Calculate the angle needed to point at the predicted position
        float angle = Mathf.Atan2(directionToPredictedPosition.y, directionToPredictedPosition.x) * Mathf.Rad2Deg + 90;

        // Smoothly rotate the cannon towards the predicted position
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if the player is within shooting distance
        if (directionToPlayer.magnitude <= shootingDistance)
        {
            timeSinceLastShot += Time.deltaTime;

            // Check if enough time has passed to shoot again
            if (timeSinceLastShot >= shootingInterval)
            {
                ShootCannonball(directionToPredictedPosition);
                timeSinceLastShot = 0f;  // Reset the shooting timer
            }
        }
    }

    void ShootCannonball(Vector3 direction)
    {
        // Instantiate the cannonball at the cannon's position and rotation
        GameObject cannonball = Instantiate(cannonballPrefab, transform.position, transform.rotation);

        // Get the Rigidbody2D component of the cannonball
        Rigidbody2D cannonballRigidbody = cannonball.GetComponent<Rigidbody2D>();

        // Set the velocity of the cannonball to shoot towards the predicted position
        cannonballRigidbody.velocity = direction.normalized * projectileSpeed;
    }
}
