using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrowingCurvedLine : MonoBehaviour
{
    public Transform playerTransform; // The player or the target position
    public int maxPoints = 100; // Maximum number of points in the line
    public float maxDistance = 20f; // Maximum distance from the player
    public float growthRate = 0.5f; // Rate at which the line grows
    public float curveAmount = 1f; // Amount of curvature

    private LineRenderer lineRenderer;
    private Vector3[] points;
    private int currentPointCount = 0;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        points = new Vector3[maxPoints];
        lineRenderer.positionCount = maxPoints;
        GenerateLine();
    }

    void Update()
    {
        if (currentPointCount < maxPoints)
        {
            GrowLine();
        }
    }

    void GenerateLine()
    {
        points[0] = playerTransform.position;
        points[1] = playerTransform.position + Vector3.up;

        for (int i = 2; i < maxPoints; i++)
        {
            points[i] = points[i - 1] + GenerateNextPoint();
        }

        lineRenderer.SetPositions(points);
    }

    Vector3 GenerateNextPoint()
    {
        Vector3 direction = Vector3.up;
        direction += new Vector3(
            Random.Range(-curveAmount, curveAmount),
            Random.Range(0.5f, 1f),  // Ensure movement in the positive y direction
            Random.Range(-curveAmount, curveAmount)
        ).normalized;

        // Ensure the new point is within maxDistance from the player
        Vector3 nextPoint = points[currentPointCount] + direction * growthRate;
        float distanceFromPlayer = Vector3.Distance(nextPoint, playerTransform.position);
        if (distanceFromPlayer > maxDistance)
        {
            nextPoint = playerTransform.position + (nextPoint - playerTransform.position).normalized * maxDistance;
        }

        return nextPoint - points[currentPointCount];
    }

    void GrowLine()
    {
        // Update the line
        currentPointCount++;
        if (currentPointCount >= maxPoints)
            return;

        points[currentPointCount] = points[currentPointCount - 1] + GenerateNextPoint();
        lineRenderer.SetPositions(points);
    }
}
