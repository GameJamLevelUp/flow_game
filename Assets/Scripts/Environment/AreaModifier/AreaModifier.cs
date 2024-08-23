using System.Collections.Generic;
using UnityEngine;

public abstract class AreaModifier : MonoBehaviour
{
    public abstract void OnEngage();

    public GameObject activator; // The activator area
    public GameObject detection; // The detection area
    public GameObject effective;
    public float slowdownRange = 15f;
    public float slowdownFactor = 0.5f; // The target time scale when slowed down
    public float slowdownSpeed = 2f; // Speed of transitioning to and from the slowdown effect
    public float normalTimeScale = 1f; // Normal time scale

    public GameObject player;
    private TimeController timeController;

    private bool isInDetectionRange = false;
    private bool hasEngaged = false; // Flag to track if OnEngage has been called

    public float deleteRadius = 100f; // Radius within which to check for other instances

    private void Start()
    {
        // Check if any other FogArea instances are within the deleteRadius
        CheckAndDestroyNearbyInstances();

        // Find the player object
        player = GameObject.FindGameObjectWithTag("Player");
        timeController = GameObject.FindAnyObjectByType<TimeController>();
    }

    private void Update()
    {
        if (player != null)
        {
            // Check the player's presence in detection and activator areas
            bool playerInDetection = IsPlayerWithinArea(detection);
            bool playerInActivator = IsPlayerWithinArea(activator);

            // Handle slowdown effect for the detection area
            if (playerInDetection && !isInDetectionRange)
            {
                isInDetectionRange = true;
                timeController.targetTimeScaleEffects = slowdownFactor;
            }
            else if (!playerInDetection && isInDetectionRange)
            {
                isInDetectionRange = false;
                timeController.targetTimeScaleEffects = 1f;
            }

            // Handle engagement for the activator area
            if (playerInActivator && !hasEngaged)
            {
                OnEngage();
                hasEngaged = true; // Set flag to true after calling OnEngage
            }
        }
    }

    private void CheckAndDestroyNearbyInstances()
    {
        // Find all instances of FogArea in the scene
        AreaModifier[] allFogAreas = FindObjectsOfType<AreaModifier>();

        // Check distance to other instances
        foreach (AreaModifier otherFogArea in allFogAreas)
        {
            if (otherFogArea != this) // Ignore self
            {
                float distance = Vector2.Distance(transform.position, otherFogArea.transform.position);
                if (distance <= deleteRadius)
                {
                    // Destroy this instance if another instance is within deleteRadius
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    public bool IsPlayerWithinArea(GameObject area)
    {
        if (area != null && player != null)
        {
            // Get the area’s Transform component
            Transform areaTransform = area.transform;

            // Get the size of the area (assuming it's a 2D rectangle)
            Vector2 size = new Vector2(
                areaTransform.localScale.x,
                areaTransform.localScale.y
            );

            // Calculate half the size (for easier calculations)
            Vector2 halfSize = size * 0.5f;

            // Calculate the corners of the rectangle in local space
            Vector2 topLeft = new Vector2(-halfSize.x, halfSize.y);
            Vector2 topRight = new Vector2(halfSize.x, halfSize.y);
            Vector2 bottomLeft = new Vector2(-halfSize.x, -halfSize.y);
            Vector2 bottomRight = new Vector2(halfSize.x, -halfSize.y);

            // Apply the rotation to the corners
            Quaternion rotation = areaTransform.rotation;
            topLeft = rotation * topLeft;
            topRight = rotation * topRight;
            bottomLeft = rotation * bottomLeft;
            bottomRight = rotation * bottomRight;

            // Translate corners to world space
            topLeft += (Vector2)areaTransform.position;
            topRight += (Vector2)areaTransform.position;
            bottomLeft += (Vector2)areaTransform.position;
            bottomRight += (Vector2)areaTransform.position;

            // Create a bounding rectangle from the corners
            Vector2 min = Vector2.Min(Vector2.Min(topLeft, topRight), Vector2.Min(bottomLeft, bottomRight));
            Vector2 max = Vector2.Max(Vector2.Max(topLeft, topRight), Vector2.Max(bottomLeft, bottomRight));

            // Check if the player's position is within the bounding rectangle
            Vector2 playerPosition = (Vector2)player.transform.position;
            return playerPosition.x >= min.x && playerPosition.x <= max.x &&
                   playerPosition.y >= min.y && playerPosition.y <= max.y;
        }

        return false;
    }
}
