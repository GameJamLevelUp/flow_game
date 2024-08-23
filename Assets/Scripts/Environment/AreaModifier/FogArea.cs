using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FogArea : AreaModifier
{
    public GameObject prefabToSpawn; // The prefab to spawn
    public float spawnRange = 5f; // Range within which to spawn prefabs
    public float minSpacing = 1f; // Minimum spacing between prefabs
    public float transitionDuration = 2f; // Duration of the color transition

    private Vector2 startPosition;
    private Vector2 endPosition;
    private Light2D globalLight;
    private Color originalLightColor;

    public override void OnEngage()
    {
        Debug.Log("engaged");

        globalLight = FindObjectOfType<Light2D>();
        if (globalLight != null)
        {
            originalLightColor = globalLight.color;
        }

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
            SpawnPrefabsInRectangle(point1, point2);
        }

        // Start the light transition coroutine
        StartCoroutine(LerpLightColor(Color.gray, transitionDuration));

        // Check if the player is past the end position
        if (player.transform.position.y -  endPosition.y > spawnRange)
        {
            ReverseDarkness();
        }
    }

    private void SpawnPrefabsInRectangle(Vector2 point1, Vector2 point2)
    {
        Vector2 direction = (point2 - point1).normalized;
        float length = Vector2.Distance(point1, point2);

        HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

        // Generate 10 prefabs within the rectangle
        for (int i = 0; i < 3; i++)
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
            Instantiate(prefabToSpawn, finalPosition, Quaternion.identity);
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

    private IEnumerator LerpLightColor(Color targetColor, float duration)
    {
        if (globalLight == null) yield break;

        Color startColor = globalLight.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            globalLight.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        globalLight.color = targetColor;
    }

    private void ReverseDarkness()
    {
        Debug.Log("Reversing darkness");
        // Start the coroutine to revert the light color
        StartCoroutine(LerpLightColor(originalLightColor, transitionDuration));
    }
}
