using System.Collections.Generic;
using UnityEngine;

public class RandomizeDirectionFromLine : MonoBehaviour
{
    public PathGenerator path;  // Reference to the LineRenderer
    public bool alignWithFlowDirection = false;  // Align with flow direction or randomize
    [Range(-360f, 0f)] public float minAngle = 0f;  // Minimum angle for randomization
    [Range(0f, 360f)] public float maxAngle = 360f;  // Maximum angle for randomization
    public float degreesOffset = 0f;  // Offset in degrees for the alignment

    void Start()
    {
       path = GameObject.FindAnyObjectByType<PathGenerator>();

        // Find the closest 3 points on the LineRenderer
        List<Vector2> closestPoints = FindClosestPoints();

        if (alignWithFlowDirection && closestPoints.Count == 3)
        {
            // Align with the flow direction based on the closest points
            AlignWithFlowDirection(closestPoints);
        }
        else
        {
            // Randomize the direction within the specified angle range
            RandomizeRotation(0f);
        }
    }

    List<Vector2> FindClosestPoints()
    {
        List<Vector2> closestPoints = new List<Vector2>();
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < path.points.Count - 2; i++)
        {
            // Get 3 consecutive points
            Vector2 point1 = path.points[1];
            Vector2 point2 = path.points[i + 1];
            Vector2 point3 = path.points[i + 2];

            // Calculate the average position of these 3 points
            Vector2 averagePoint = (point1 + point2 + point3) / 3f;

            // Calculate the distance from the object to this average point
            float distance = Vector2.Distance(transform.position, averagePoint);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoints.Clear();
                closestPoints.Add(point1);
                closestPoints.Add(point2);
                closestPoints.Add(point3);
            }
        }

        return closestPoints;
    }

    void AlignWithFlowDirection(List<Vector2> points)
    {
        // Calculate the flow direction based on the closest points
        Vector2 flowDirection = (points[2] - points[0]).normalized;

        // Calculate the angle based on the flow direction
        float angle = Mathf.Atan2(flowDirection.y, flowDirection.x) * Mathf.Rad2Deg;

        // Apply the rotation with the offset and randomization
        transform.rotation = Quaternion.Euler(0, 0, angle + degreesOffset);

        // Apply additional random rotation
        RandomizeRotation(angle + degreesOffset);
    }

    void RandomizeRotation(float baseAngle)
    {
        // Generate a random angle within the specified range
        float randomAngle = Random.Range(minAngle, maxAngle);

        // Apply the rotation as an offset to the base angle
        transform.rotation = Quaternion.Euler(0, 0, baseAngle + randomAngle);
    }

    // Ensure minAngle is less than or equal to maxAngle
    void OnValidate()
    {
        if (minAngle > maxAngle)
        {
            minAngle = maxAngle;
        }
    }
}
