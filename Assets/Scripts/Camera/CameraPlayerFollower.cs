using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Rigidbody2D target;  // The target object to follow
    public float smoothSpeed = 0.125f;  // Smoothing factor
    public Vector3 offset;  // Offset from the target position

    public Camera camera;

    private float currentOrthoSize; // Store the current orthographic size

    void Start()
    {
        // Initialize the currentOrthoSize with the camera's initial orthographic size
        currentOrthoSize = camera.orthographicSize;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position with offset
        Vector3 desiredPosition = target.transform.position + offset * (1f + target.velocity.magnitude / 2f);

        // Calculate the desired orthographic size based on the target's velocity
        float desiredOrthoSize = 5 * (1f + target.velocity.magnitude / 20f);

        // Smoothly interpolate the orthographic size
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, desiredOrthoSize, smoothSpeed * Time.deltaTime);

        // Update the position of the object
        transform.position = desiredPosition;
    }
}
