using System.Collections.Generic;
using UnityEngine;

public abstract class AreaModifier : MonoBehaviour
{
    public abstract void OnEngage();

    public abstract void OnUpdate();
    public abstract void OnStart();

    public GameObject activator; // The activator area
    public GameObject detection; // The detection area
    public GameObject effective;
    public float slowdownRange = 15f;
    public float slowdownFactor = 2.5f; // The target time scale when slowed down
    public float slowdownSpeed = 2f; // Speed of transitioning to and from the slowdown effect
    public float normalTimeScale = 1f; // Normal time scale

    public GameObject player;
    private TimeController timeController;

    private bool isInDetectionRange = false;
    public bool hasEngaged = false; // Flag to track if OnEngage has been called

    public float deleteRadius = 100f; // Radius within which to check for other instances

    private void Start()
    {
        // Check if any other FogArea instances are within the deleteRadius
        CheckAndDestroyNearbyInstances();

        // Find the player object
        player = GameObject.FindGameObjectWithTag("Player");
        timeController = GameObject.FindAnyObjectByType<TimeController>();
        OnStart();
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
                float factor = slowdownFactor / player.GetComponent<Rigidbody2D>().velocity.magnitude;
                timeController.targetTimeScaleEffects = Mathf.Min(1f, factor);
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
        OnUpdate();
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
            // Get the area's Transform component
            Transform areaTransform = area.transform;

            // Calculate the local space size of the area using its localScale (assuming the area is a 2D rectangle)
            Vector2 size = new Vector2(
                1f,
                1f
            );

            if (area == activator)
            {
                size = new Vector2(
                    2.5f,
                    1f
                );
            }

            // Calculate half the size for boundary checks
            Vector2 halfSize = size * 0.5f;

            // Convert the player's position to the local space of the area
            Vector2 localPlayerPosition = areaTransform.InverseTransformPoint(player.transform.position);

            // Check if the player's local position is within the bounds of the rectangle
            return localPlayerPosition.x >= -halfSize.x && localPlayerPosition.x <= halfSize.x &&
                localPlayerPosition.y >= -halfSize.y && localPlayerPosition.y <= halfSize.y;
        }

        return false;
    }
void OnDrawGizmos()
{
    if (effective != null && player != null)
    {
        // Get the area's Transform component
        Transform areaTransform = effective.transform;

        // Calculate the world space size of the area using its lossyScale
        Vector2 size = new Vector2(
            areaTransform.lossyScale.x,
            areaTransform.lossyScale.y
        );

        // Calculate half the size for easier calculations
        Vector2 halfSize = size * 0.5f;

        // Calculate the four corners of the rectangle in local space
        Vector3 bottomLeft = new Vector3(-0.5f,-0.5f, 0f);
        Vector3 bottomRight = new Vector3(0.5f, -0.5f, 0f);
        Vector3 topLeft = new Vector3(-0.5f, 0.5f, 0f);
        Vector3 topRight = new Vector3(0.5f, 0.5f, 0f);

        // Convert local corners to world space
        bottomLeft = areaTransform.TransformPoint(bottomLeft);
        bottomRight = areaTransform.TransformPoint(bottomRight);
        topLeft = areaTransform.TransformPoint(topLeft);
        topRight = areaTransform.TransformPoint(topRight);

        // Draw the rectangle
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // Draw the player's position as a sphere
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(player.transform.position, 0.5f);
    }
}


}
