using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OffsetLineRenderer : MonoBehaviour
{
    public LineRenderer originalLineRenderer; // The LineRenderer from which to get points
    public LineRenderer offsetLineRenderer;   // The LineRenderer to which the offset line will be drawn
    public float offset = 1.0f;               // The offset distance from the original line

    private void Start()
    {
        // Initialize the offset line renderer
        if (offsetLineRenderer == null)
        {
            Debug.LogError("Offset LineRenderer is not assigned!");
            return;
        }
        offsetLineRenderer.positionCount = originalLineRenderer.positionCount;
    }

    private void Update()
    {
        if (originalLineRenderer == null || offsetLineRenderer == null)
        {
            return;
        }

        // Get the points from the original LineRenderer
        Vector3[] originalPoints = new Vector3[originalLineRenderer.positionCount];
        originalLineRenderer.GetPositions(originalPoints);

        // Calculate offset points
        Vector3[] offsetPoints = new Vector3[originalPoints.Length];
        for (int i = 0; i < originalPoints.Length - 1; i++)
        {
            Vector3 start = originalPoints[i];
            Vector3 end = originalPoints[i + 1];
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, direction.z);
            offsetPoints[i] = start + perpendicular * offset;
        }
        offsetPoints[offsetPoints.Length - 1] = originalPoints[originalPoints.Length - 1] + (originalPoints[originalPoints.Length - 1] - originalPoints[originalPoints.Length - 2]).normalized * offset;

        // Update the offset LineRenderer with new points
        offsetLineRenderer.positionCount = offsetPoints.Length;
        offsetLineRenderer.SetPositions(offsetPoints);
    }

    private void OnDrawGizmos()
    {
        if (originalLineRenderer == null || offsetLineRenderer == null)
        {
            return;
        }

        // Draw original line
        Gizmos.color = Color.green;
        DrawLineGizmos(originalLineRenderer);

        // Draw offset line
        Gizmos.color = Color.red;
        DrawLineGizmos(offsetLineRenderer);
    }

    private void DrawLineGizmos(LineRenderer lineRenderer)
    {
        Vector3[] points = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(points);

        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
}
