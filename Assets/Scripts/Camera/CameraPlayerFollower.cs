using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;  // The target object to follow
    public float smoothSpeed = 0.125f;  // Smoothing factor
    public Vector3 offset;  // Offset from the target position

    void LateUpdate()
    {
        if (target == null)
            return;

        // Define the desired position with the offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the position of the object
        transform.position = smoothedPosition;

        // Optional: Uncomment the following line if you want the object to look at the target
        // transform.LookAt(target);
    }
}
