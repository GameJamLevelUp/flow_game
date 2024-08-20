using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AestheticItem
{
    public GameObject prefab; // The prefab/GameObject to spawn
    public float NormalizingSpawnDistance = 2500f;
    public AnimationCurve spawnChanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // The curve to define spawn chance

    // Method to evaluate the spawn chance at a given point
    public float EvaluateSpawnChance(float distance)
    {
        float duration = distance / NormalizingSpawnDistance;
        return spawnChanceCurve.Evaluate(duration);
    }
}

[RequireComponent(typeof(LineRenderer))]
public class PathGenerator : MonoBehaviour
{

    

    public Transform playerTransform; // The player or the target position
    public float maxDistance = 20f; // Maximum distance from the player
    public float growthRate = 0.5f; // Rate at which the line grows
    public float curveAmount = 1f; // Amount of curvature
    public float curveFrequency = 1f; // Frequency of the curve
    public float curveAmplitude = 1f; // Amplitude of the curve
    public List<AestheticItem> aestheticItems; // List of aesthetic items
    public List<AestheticItem> attachableItems; // List of attachable items
    public float spawnRadius = 2f; // Radius within which to spawn the attachable

    private LineRenderer lineRenderer;
    public List<Vector3> points;
    private Vector3 lastPointPosition;
    private float timeElapsed = 0f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        points = new List<Vector3>
        {
            playerTransform.position
        };
        lastPointPosition = playerTransform.position;

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    void Update()
    {
        float distanceFromPlayer = Vector3.Distance(lastPointPosition, playerTransform.position);

        if (distanceFromPlayer < maxDistance)
        {
            GrowLine();
        }
    }

    void GrowLine()
    {
        Vector3 nextPoint = GenerateNextPoint();
        if (nextPoint.magnitude > growthRate)
        {
            Vector3 newPoint = lastPointPosition + nextPoint;
            points.Add(newPoint);
            lastPointPosition = newPoint; // Update last point position

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());

            // Chance to spawn an attachable item
            SpawnAttachable(newPoint);
            SpawnAesthetic(newPoint);
        }
    }

    Vector3 GenerateNextPoint()
    {
        

        // Generate a sinusoidal curve effect
        float curveX = Mathf.Sin(lastPointPosition.y * Mathf.Deg2Rad * curveFrequency) * curveAmplitude;
        float curveZ = Mathf.Cos(lastPointPosition.y * Mathf.Deg2Rad * curveFrequency) * curveAmplitude;

        // Direction with sinusoidal variations for more natural curves
        Vector3 direction = Vector3.up;
        direction += new Vector3(
            curveX,
            UnityEngine.Random.Range(5f, 10f),  // Ensure movement in the positive y direction
            curveZ
        ).normalized;

        // Ensure the new point is within maxDistance from the player
        Vector3 nextPoint = lastPointPosition + direction * growthRate;
        float distanceFromPlayer = Vector3.Distance(nextPoint, playerTransform.position);
        if (distanceFromPlayer > maxDistance)
        {
            nextPoint = playerTransform.position + (nextPoint - playerTransform.position).normalized * maxDistance;
        }

        return nextPoint - lastPointPosition;
    }

    private void SpawnAttachable(Vector3 spawnPosition)
    {
        if (attachableItems != null && attachableItems.Count > 0)
        {
            // Randomly select an AestheticItem based on their spawn chances
            float randomValue = Random.value;
            float cumulativeChance = 0f;

            foreach (var item in attachableItems)
            {
                cumulativeChance += item.EvaluateSpawnChance(playerTransform.position.y);
                if (randomValue <= cumulativeChance)
                {
                    // Random position within the spawn radius
                    Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                    randomOffset.y = 0; // Optional: Keep the attachable on the same level as the line

                    // Instantiate the selected item at the calculated position
                    if (item.prefab != null)
                    {
                        Instantiate(item.prefab, spawnPosition + randomOffset, Quaternion.identity);
                    }
                    break;
                }
            }
        }
    }

    private void SpawnAesthetic(Vector3 spawnPosition)
    {
        if (aestheticItems != null && aestheticItems.Count > 0)
        {
            // Randomly select an AestheticItem based on their spawn chances
            float randomValue = Random.value;
            float cumulativeChance = 0f;

            foreach (var item in aestheticItems)
            {
                cumulativeChance += item.EvaluateSpawnChance(1);
                if (randomValue <= cumulativeChance)
                {
                    // Random position within the spawn radius
                    Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                    randomOffset.y = 0; // Optional: Keep the attachable on the same level as the line

                    // Instantiate the selected item at the calculated position
                    if (item.prefab != null)
                    {
                        Instantiate(item.prefab, spawnPosition + randomOffset, Quaternion.identity);
                    }
                    break;
                }
            }
        }
    }

    // Draw debug lines in the Editor
    private void OnDrawGizmos()
    {
        if (points == null || points.Count < 2)
            return;

        Gizmos.color = Color.red;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
}
