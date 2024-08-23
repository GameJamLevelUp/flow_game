using System.Collections.Generic;
using UnityEngine;

public class AlignToLineRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;  // Reference to the LineRenderer
    public float moveDistance = 0.5f;  // Distance to move along the normal direction
    public bool debugNormals = false;  // Option to visualize normals in the editor
    public float addDirection = 0f;
    public Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position;

        lineRenderer = GameObject.FindAnyObjectByType<PlayerFlowMover>().GetComponent<LineRenderer>();

        // Find the closest 3 points on the LineRenderer
        List<Vector2> closestPoints = FindClosestPoints();

        if (closestPoints.Count == 3)
        {
            // Calculate the normal direction between the 3 points
            Vector2 normalDirection = CalculateNormalDirection(closestPoints);

            // Determine which side of the line the object is on
            Vector2 pointToObj = (Vector2)transform.position - closestPoints[1];
            float side = Mathf.Sign(Vector2.Dot(pointToObj, normalDirection));

            // Move the object in the correct normal direction based on the side
            transform.position += (Vector3)(normalDirection.normalized * moveDistance * side);

            // Rotate the object to point in the opposite direction of the normal
            RotateToOppositeNormal(normalDirection * side);
        }
    }

    List<Vector2> FindClosestPoints()
    {
        List<Vector2> closestPoints = new List<Vector2>();
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < lineRenderer.positionCount - 2; i++)
        {
            // Get 3 consecutive points
            Vector2 point1 = lineRenderer.GetPosition(i);
            Vector2 point2 = lineRenderer.GetPosition(i + 1);
            Vector2 point3 = lineRenderer.GetPosition(i + 2);

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

    Vector2 CalculateNormalDirection(List<Vector2> points)
    {
        // Calculate the vectors between the 3 points
        Vector2 vector1 = points[1] - points[0];
        Vector2 vector2 = points[2] - points[1];

        // Calculate the perpendicular direction to get the normal direction
        Vector2 normalDirection = Vector2.Perpendicular(vector1 + vector2);

        // Optionally visualize the normal in the editor
        if (debugNormals)
        {
            Debug.DrawRay((points[0] + points[1] + points[2]) / 3f, normalDirection, Color.green, 5f);
        }

        return normalDirection;
    }

    void RotateToOppositeNormal(Vector2 normalDirection)
    {
        // Calculate the angle to rotate the object to face the opposite direction of the normal
        float angle = Mathf.Atan2(-normalDirection.y, -normalDirection.x) * Mathf.Rad2Deg;

        // Apply the rotation
        transform.rotation = Quaternion.Euler(0, 0, angle + addDirection);
    }
}
