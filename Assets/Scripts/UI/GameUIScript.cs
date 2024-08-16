using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public RectTransform healthContainer; // The container holding the health segments
    public CanvasGroup containerCanvasGroup; // CanvasGroup to control transparency of the container

    private Image[] healthSegments; // Array of health segments represented by UI images

    void Start()
    {
        // Get all Image components that are children of the healthContainer
        healthSegments = healthContainer.GetComponentsInChildren<Image>();
        
        // Optionally initialize the health display here
    }

    // Call this method to handle damage and update the health UI
    private int damageTaken = 0;
    public void ReceiveDamage()
    {
        if (damageTaken < 0 || damageTaken >= healthSegments.Length)
        {
            Debug.LogWarning("Invalid segment index");
            return;
        }
        // Make the specified health segment transparent
        SetHealthSegmentTransparency(damageTaken, 0.25f); 

        FollowTarget cameraScript = GameObject.FindAnyObjectByType<FollowTarget>();

        cameraScript.Shake(0.2f, 0.1f);

        damageTaken++;

        // Optionally, adjust container transparency if needed
        // containerCanvasGroup.alpha = Mathf.Max(0, containerCanvasGroup.alpha - 0.1f);
    }

    // Set the transparency of a specific health segment
    private void SetHealthSegmentTransparency(int index, float alpha)
    {
        Color color = healthSegments[healthSegments.Length - 1 - index].color;
        color.a = alpha;
        healthSegments[healthSegments.Length - 1 - index].color = color;
    }

    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI speedText;
    public Animator animator;
    private float prevDistance = 0;
    public void SetDistance(float distance, float speed)
    {
        speedText.text = $"{speed:F0}m/s";

        if (distance > prevDistance + 0.1f) {

            prevDistance = distance;

            float dividedDistance = distance / 1000;
            // Update the text to show distance rounded to 2 decimal places
            distanceText.text = $"{dividedDistance:F2} km";

            // Set the trigger on the animator to fire the animation
            animator.SetTrigger("TriggerDistanceGrow");
        }
    }

    private float targetTimeScale = 1.0f; // Target time scale
    private float timeScaleSpeed = 5.0f; // Speed of transition

    void Update()
    {
        // Check if the space key is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            targetTimeScale = 0.3f; // Set target game speed to 30%
        }
        else
        {
            targetTimeScale = 1.0f; // Set target game speed to 100%
        }

        // Smoothly interpolate the time scale
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.deltaTime * timeScaleSpeed);
    }
}
