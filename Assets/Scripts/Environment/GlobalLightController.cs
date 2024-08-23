using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightController : MonoBehaviour
{
    public Light2D light2D; // Reference to the Light2D component
    public Color targetColor = Color.white; // The target color to lerp to
    public float transitionDuration = 2f; // Duration of the color transition in seconds

    private Color initialColor; // The initial color of the light

    private void Start()
    {
        if (light2D == null)
        {
            light2D = GetComponent<Light2D>();
        }

        // Ensure the initial color is set at the start
        if (light2D != null)
        {
            initialColor = light2D.color;
        }
    }

    public void LerpToColor(Color newColor)
    {
        if (light2D != null)
        {
            targetColor = newColor;
            StopCoroutine(LerpLightColor());
            StartCoroutine(LerpLightColor());
        }
    }

    public void Reset()
    {
        targetColor = initialColor;
        StopCoroutine(LerpLightColor());
        StartCoroutine(LerpLightColor());
    }

    private IEnumerator LerpLightColor()
    {
        isTransitioning = true;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            light2D.color = Color.Lerp(initialColor, targetColor, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set
        light2D.color = targetColor;
        initialColor = targetColor;

        isTransitioning = false;
    }
}
