using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrowingCurvedLine : MonoBehaviour
{
    public Transform playerTransform; // The player or the target position
    public float maxDistance = 20f; // Maximum distance from the player
    public float growthRate = 0.5f; // Rate at which the line grows
    public float curveAmount = 1f; // Amount of curvature
    public float pointInterval = 1f; // Distance interval between new points
    public float curveFrequency = 1f; // Frequency of the curve
    public float curveAmplitude = 1f; // Amplitude of the curve
    public GameObject spawnAttachable; // The attachable prefab
    public float spawnRadius = 2f; // Radius within which to spawn the attachable

    private LineRenderer lineRenderer;
    public List<Vector3> points;
    private float distanceTraveled = 0f;
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

            // Chance to spawn the attachable
            if (Random.value <= 0.25f) // 25% chance
            {
                SpawnAttachable(newPoint);
            }
        }
    }

    Vector3 GenerateNextPoint()
    {
        timeElapsed += Time.deltaTime;

        // Generate a sinusoidal curve effect
        float curveX = Mathf.Sin(timeElapsed * curveFrequency) * curveAmplitude;
        float curveZ = Mathf.Cos(timeElapsed * curveFrequency) * curveAmplitude;

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
        if (spawnAttachable != null)
        {
            // Random position within the spawn radius
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0; // Optional: Keep the attachable on the same level as the line

            // Instantiate the attachable at the calculated position
            Instantiate(spawnAttachable, spawnPosition + randomOffset, Quaternion.identity);
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
