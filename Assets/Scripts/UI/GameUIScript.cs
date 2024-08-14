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
        SetHealthSegmentTransparency(damageTaken, 0.25f); // Example transparency value, 0 (invisible) to 1 (fully visible)
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
}
