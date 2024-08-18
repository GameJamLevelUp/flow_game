using UnityEngine;

public class PlayerFlowMover : MonoBehaviour
{
    public LineRenderer lineRenderer; // The LineRenderer to read points from
    public Rigidbody2D playerRb;      // The Rigidbody2D of the player
    public float pushForce = 1.0f;   // The force to apply to the player

    private void FixedUpdate()
    {
        if (lineRenderer == null || playerRb == null)
        {
            Debug.LogError("LineRenderer or Rigidbody2D is not assigned!");
            return;
        }

        // Get the position of the player
        Vector2 playerPosition = playerRb.position;

        // Find the closest point on the LineRenderer to the player
        Vector3[] linePoints = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(linePoints);

        int closestIndex = FindClosestPointIndex(playerPosition, linePoints);
        if (closestIndex == -1 || linePoints.Length < 3)
        {
            return;
        }

        // Get the previous, closest, and next points
        Vector3 previousPoint = linePoints[Mathf.Max(0, closestIndex - 1)];
        Vector3 closestPoint = linePoints[closestIndex];
        Vector3 nextPoint = linePoints[Mathf.Min(linePoints.Length - 1, closestIndex + 1)];

        // Calculate the average direction vector
        Vector3 direction = (nextPoint - previousPoint).normalized;

        // Apply force to the player in the direction of the curve
        playerRb.AddForce(direction * pushForce);
    }

    private int FindClosestPointIndex(Vector2 playerPosition, Vector3[] points)
    {
        int closestIndex = -1;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector2.Distance(playerPosition, points[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
