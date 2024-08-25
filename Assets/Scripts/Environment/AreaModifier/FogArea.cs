using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class FogArea : AreaModifier
{
    public GameObject prefabToSpawn; // The prefab to spawn
    public GameObject enemyToSpawn;
    public float spawnRange = 5f; // Range within which to spawn prefabs
    public float minSpacing = 1f; // Minimum spacing between prefabs
    public float transitionDuration = 2f; // Duration of the color transition
    public VisualEffect visualEffect; // Reference to the Visual Effect Graph component

    private Vector2 startPosition;
    private Vector2 endPosition;
    private GlobalLightController globalLight;
    private bool hasFired = false;

    public override void OnStart()
    {
        if (visualEffect != null)
        {
            visualEffect.Stop();
        }
    }

    public override void OnEngage()
    {
        hasFired = true;

        globalLight = FindObjectOfType<GlobalLightController>();

        // Find the PathGenerator script
        PathGenerator pathGenerator = FindObjectOfType<PathGenerator>();
        if (pathGenerator == null)
        {
            Debug.LogError("PathGenerator not found!");
            return;
        }

        // Get the list of points from the PathGenerator
        List<Vector3> points = pathGenerator.points;
        if (points == null || points.Count == 0)
        {
            Debug.LogError("No points found in PathGenerator!");
            return;
        }

        // Find the closest point to the player
        Vector2 closestPoint = points[0];
        float minDistance = float.MaxValue;

        for (int i = 0; i < points.Count; i++)
        {
            float distance = Vector2.Distance((Vector2)player.transform.position, points[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = points[i];
            }
        }

        // Find the index of the closest point
        int closestPointIndex = points.IndexOf(closestPoint);

        // Track start and end positions
        startPosition = points[closestPointIndex];
        endPosition = (closestPointIndex + 5 < points.Count) ? points[closestPointIndex + 5] : points[points.Count - 1];

        // Find the next 5 points
        int startIndex = Mathf.Min(closestPointIndex + 1, points.Count - 5);
        for (int i = startIndex; i < startIndex + 5; i++)
        {
            Vector2 point1 = points[i];
            Vector2 point2 = (i + 1 < points.Count) ? points[i + 1] : point1;
            SpawnPrefabsInRectangle(point1, point2, prefabToSpawn);
        }

        for (int i = startIndex; i < startIndex + 5; i++)
        {
            Vector2 point1 = points[i];
            Vector2 point2 = (i + 1 < points.Count) ? points[i + 1] : point1;
            SpawnPrefabsInRectangle(point1, point2, enemyToSpawn, 1);
        }

        // Start the light transition coroutine
        globalLight.LerpToColor(new Color(0.2f, 0.2f, 0.2f));
        // Start the visual effect when the player enters the area
        if (visualEffect != null)
        {
            visualEffect.Play();
        }
    }

    public override void OnUpdate() 
    {
        // Update the visual effect's player's position if it's active
        if (visualEffect != null && visualEffect.isActiveAndEnabled)
        {
            UpdatePlayerPosition();
        }
        // Check if the player is past the end position
        if (!IsPlayerWithinArea(effective) && !IsPlayerWithinArea(detection) && hasFired)
        {
            ReverseDarkness();
            Destroy(gameObject);
        }
    
    }

    private void UpdatePlayerPosition()
    {
        // Assuming the player's position is passed to the graph as a Vector3
        Vector3 playerPosition = GetPlayerPosition(); // Implement this method to get the player's position

        playerPosition.z = 0;

        // Update the position parameter in the visual effect graph
        visualEffect.SetVector3("Player Position", playerPosition);
    }

   private Vector3 GetPlayerPosition()
    {
        // Get the player's position in world space
        Vector3 playerWorldPosition = player.transform.position;

        // Convert the player's position to local space relative to this GameObject
        Vector3 relativePosition = effective.transform.InverseTransformPoint(playerWorldPosition);

        return relativePosition;
    }


    private void SpawnPrefabsInRectangle(Vector2 point1, Vector2 point2, GameObject item, int maxAmount = 3)
    {
        Vector2 direction = (point2 - point1).normalized;
        float length = Vector2.Distance(point1, point2);

        HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

        // Generate 10 prefabs within the rectangle
        for (int i = 0; i < maxAmount; i++)
        {
            Vector2 finalPosition;
            do
            {
                // Pick a random position along the length of the rectangle
                float randomLength = Random.Range(0, length);
                Vector2 pointAlongLine = point1 + direction * randomLength;

                // Calculate a random offset perpendicular to the direction
                Vector2 perpendicularOffset = new Vector2(-direction.y, direction.x).normalized * Random.Range(-spawnRange / 2, spawnRange / 2);

                // Calculate the final position within the rectangle
                finalPosition = pointAlongLine + perpendicularOffset;
            }
            while (IsPositionTooClose(finalPosition, occupiedPositions));

            // Add the position to the occupied set
            occupiedPositions.Add(finalPosition);

            // Instantiate the prefab at the final position
            Instantiate(item, finalPosition, Quaternion.identity);
        }
    }

    private bool IsPositionTooClose(Vector2 position, HashSet<Vector2> occupiedPositions)
    {
        foreach (var occupiedPosition in occupiedPositions)
        {
            if (Vector2.Distance(position, occupiedPosition) < minSpacing)
            {
                return true;
            }
        }
        return false;
    }

    private void ReverseDarkness()
    {
        globalLight.Reset();
    }
}
