using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour
{
    public Rigidbody2D target;  // The target object to follow
    public float smoothSpeed = 0.125f;  // Smoothing factor
    public Vector3 offset;  // Offset from the target position
    new public Camera camera;   // Reference to the camera

    private float currentOrthoSize; // Store the current orthographic size
    private Vector3 originalPosition; // Store the original position of the camera

    // Shake parameters
    public float shakeMagnitude = 0.1f; // Magnitude of the shake
    public float shakeDuration = 0f; // Duration of the shake

    void Start()
    {
        // Initialize the currentOrthoSize with the camera's initial orthographic size
        currentOrthoSize = camera.orthographicSize;
        originalPosition = transform.position; // Store the original position
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

        Vector3 shakeOffset = new Vector3();
        if (shakeDuration > 0)
        {
            shakeOffset = Random.insideUnitCircle * shakeMagnitude / (1 - shakeDuration / 0.1f);
            shakeDuration -= Time.deltaTime;
        }

        transform.position = desiredPosition + shakeOffset;

    }

    public void Shake(float magnitude, float duration)
    {
        shakeMagnitude = magnitude;
        shakeDuration += duration;
    }

}