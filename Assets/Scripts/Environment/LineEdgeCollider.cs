using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineEdgeCollider : MonoBehaviour
{
    public bool useLeftSide = true; // Toggle to choose left or right side of the line
    public float colliderOffset = 0.1f; // Distance from the line to the collider
    public PhysicsMaterial2D edgeMaterial; // Physics material for the collider

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

        if (edgeMaterial != null)
        {
            edgeCollider.sharedMaterial = edgeMaterial;
        }
    }

    public void UpdateCollider()
    {
        if (lineRenderer == null || edgeCollider == null)
        {
            return;
        }

        Vector2[] linePoints = new Vector2[lineRenderer.positionCount];
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            linePoints[i] = (Vector2)lineRenderer.GetPosition(i);
        }

        Vector2[] colliderPoints = new Vector2[linePoints.Length];
        for (int i = 0; i < linePoints.Length; i++)
        {
            Vector2 offset = new Vector2(1, 0) * colliderOffset;
            if (!useLeftSide)
            {
                offset = -offset;
            }
            colliderPoints[i] = linePoints[i] + offset;
        }

        edgeCollider.points = colliderPoints;
    }
}
