using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LinePolygonCollider : MonoBehaviour
{
    public bool useLeftSide = true; // Toggle to choose left or right side of the line
    public PhysicsMaterial2D polygonMaterial; // Physics material for the collider

    private LineRenderer lineRenderer;
    private PolygonCollider2D polygonCollider;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        polygonCollider = gameObject.AddComponent<PolygonCollider2D>();

        if (polygonMaterial != null)
        {
            polygonCollider.sharedMaterial = polygonMaterial;
        }

        UpdateCollider();
    }

    public void UpdateCollider()
    {
        if (lineRenderer == null || polygonCollider == null)
        {
            return;
        }

        Vector3[] linePositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(linePositions);

        float lineWidth = lineRenderer.startWidth; // Assuming constant width for the entire line
        float halfWidth = lineWidth / 2f;

        Vector2[] colliderPoints = new Vector2[linePositions.Length * 2];

        for (int i = 0; i < linePositions.Length; i++)
        {
            Vector2 linePos = (Vector2)linePositions[i];
            Vector2 perpendicularOffset;

            if (i < linePositions.Length - 1)
            {
                Vector2 direction = ((Vector2)linePositions[i + 1] - linePos).normalized;
                perpendicularOffset = new Vector2(-direction.y, direction.x) * halfWidth;
            }
            else
            {
                Vector2 prevPos = (Vector2)linePositions[i];
                if (i != 0)
                {
                    prevPos = (Vector2)linePositions[i - 1];
                }
                Vector2 direction = (linePos - prevPos).normalized;
                perpendicularOffset = new Vector2(-direction.y, direction.x) * halfWidth;
            }

            if (!useLeftSide)
            {
                perpendicularOffset = -perpendicularOffset;
            }

            colliderPoints[i] = linePos + perpendicularOffset; // Offset for one side
            colliderPoints[colliderPoints.Length - 1 - i] = linePos - perpendicularOffset; // Offset for the other side
        }

        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, colliderPoints);
    }
}
