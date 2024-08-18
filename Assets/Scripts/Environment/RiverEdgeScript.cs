using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class OffsetLineRenderer : MonoBehaviour
{
    public LineRenderer originalLineRenderer; // The LineRenderer from which to get points
    public LineRenderer offsetLineRenderer;   // The LineRenderer to which the offset line will be drawn
    public float offset = 1.0f;               // The offset distance from the original line
    public float noiseAmount = 0.1f;          // The amount of noise to add
    public new LinePolygonCollider collider;

    private List<Vector3> noiseList = new List<Vector3>(); // List to store noise values

    private void Start()
    {
        // Initialize the offset line renderer
        if (offsetLineRenderer == null)
        {
            Debug.LogError("Offset LineRenderer is not assigned!");
            return;
        }

        collider = transform.GetComponent<LinePolygonCollider>();

        // Initialize noiseList with current number of positions
        InitializeNoiseList();
    }

    private void InitializeNoiseList()
    {
        // Generate unique noise for each position in the LineRenderer
        noiseList.Clear();
        for (int i = 0; i < originalLineRenderer.positionCount; i++)
        {
            noiseList.Add(new Vector3(
                Random.Range(-noiseAmount, noiseAmount),
                Random.Range(-noiseAmount, noiseAmount),
                Random.Range(-noiseAmount, noiseAmount)
            ));
        }
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

        // Make sure noiseList has enough elements
        if (noiseList.Count < originalPoints.Length)
        {
            for (int i = noiseList.Count; i < originalPoints.Length; i++)
            {
                noiseList.Add(new Vector3(
                    Random.Range(-noiseAmount, noiseAmount),
                    Random.Range(-noiseAmount, noiseAmount),
                    Random.Range(-noiseAmount, noiseAmount)
                ));
            }
        }
        else if (noiseList.Count > originalPoints.Length)
        {
            noiseList.RemoveRange(originalPoints.Length, noiseList.Count - originalPoints.Length);
        }

        // Calculate offset points with noise from the noiseList
        Vector3[] offsetPoints = new Vector3[originalPoints.Length];
        for (int i = 0; i < originalPoints.Length; i++)
        {
            Vector3 start = originalPoints[i];
            Vector3 noise = noiseList[i];

            if (i < originalPoints.Length - 1)
            {
                Vector3 end = originalPoints[i + 1];
                Vector3 direction = (end - start).normalized;
                Vector3 perpendicular = new Vector3(-direction.y, direction.x, direction.z);

                offsetPoints[i] = start + perpendicular * offset + noise;
            }
            else
            {
                // For the last point, just apply noise without adding perpendicular offset
                offsetPoints[i] = start + noise;
            }
        }

        if (collider != null)
        {
            collider.UpdateCollider();
        }
        
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
