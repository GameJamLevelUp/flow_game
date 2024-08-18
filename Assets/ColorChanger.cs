using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Rigidbody2D rb;
    TrailRenderer trailRenderer;
    public float speedThreshold = 20f;
    public Color normalTrailColor = Color.white;
    public Color fastTrailColor = Color.red;
    public float normalTrailWidth = 0.25f; // Default trail width
    public float murderModeTrailWidth = 0.5f; // Trail width in murder mode

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.startWidth = normalTrailWidth;
    }

    void Update()
    {
        if (rb != null) {
            float speed = rb.velocity.magnitude;
            CheckSpeedForTrailColorChange(speed);
        }
        
    }

    void CheckSpeedForTrailColorChange(float speed) {
        if (speed > speedThreshold) {
            SetTrailColor(fastTrailColor);
            trailRenderer.startWidth = murderModeTrailWidth;
        } else {
            SetTrailColor(normalTrailColor);
            trailRenderer.startWidth = normalTrailWidth;
        }
    }

    void SetTrailColor(Color color) {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        trailRenderer.colorGradient = gradient;
    }
}
