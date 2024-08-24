using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    private ChromaticAberration chromaticAberration; // Reference to the Chromatic Aberration effect
    public Volume volume; // Reference to the Volume component

    void Start()
    {
        // Initialize the currentOrthoSize with the camera's initial orthographic size
        currentOrthoSize = camera.orthographicSize;
        originalPosition = transform.position; // Store the original position

        // Get the Chromatic Aberration effect from the Volume
        if (volume != null && volume.profile.TryGet(out ChromaticAberration chromaticAberrationEffect))
        {
            chromaticAberration = chromaticAberrationEffect;
        }
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

        // Scale the Chromatic Aberration intensity based on the target's velocity
        if (chromaticAberration != null)
        {
            float desiredAberrationIntensity = Mathf.Clamp01((target.velocity.magnitude - 25f) / 125f);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, desiredAberrationIntensity, smoothSpeed * Time.deltaTime);
        }

        Vector3 shakeOffset = new Vector3();
        if (shakeDuration > 0)
        {
            shakeOffset = Random.insideUnitCircle * shakeMagnitude / Mathf.Max(shakeDuration / 0.1f, 0.1f);
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
