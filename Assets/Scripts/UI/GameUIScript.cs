using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public RectTransform healthContainer; // The container holding the health segments
    public CanvasGroup containerCanvasGroup; // CanvasGroup to control transparency of the container

    private Image[] healthSegments; // Array of health segments represented by UI images
    public ParticleSystem explosionParticleSystem;
    public SpriteRenderer player;
    // public GameObject skeleton;

    // Reference to the TimeController
    public TimeController timeController;

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

        if (hasDied)
        {
            return;
        }

        if (damageTaken >= healthSegments.Length - 1)
        {
            SetHealthSegmentTransparency(damageTaken, 0.25f); 
            StartCoroutine(SaveHighScoreAndSlowDown(prevDistance));
            animator.SetTrigger("TriggerDieAnimation");
            leaderboardAnimator.SetTrigger("TriggerDieAnimation");
            hasDied = true;
            timeController.hasDied = true;
            player.material.color = new Color(0.1f, 0.1f, 0.1f, 1);
            if (explosionParticleSystem != null) {
                explosionParticleSystem.Play();
            }
            return;
        }
        // Make the specified health segment transparent
        SetHealthSegmentTransparency(damageTaken, 0.25f); 

        FollowTarget cameraScript = GameObject.FindAnyObjectByType<FollowTarget>();

        cameraScript.Shake(0.2f, 0.1f);

        damageTaken++;
    }

//     void dropSkeleton()
// {
//     if (droppedSpritePrefab != null)
//     {
//         // Instantiate the sprite at the current position of the boat
//         GameObject droppedSprite = Instantiate(droppedSpritePrefab, transform.position, transform.rotation);
        
//         // Optionally, you can adjust the dropped sprite's properties or add some behavior
//         // For example, setting a different color or animation
//         SpriteRenderer droppedSpriteRenderer = droppedSprite.GetComponent<SpriteRenderer>();
//         if (droppedSpriteRenderer != null)
//         {
//             droppedSpriteRenderer.color = Color.blue;  // Change the color if desired
//         }
        
//         // The dropped sprite will stay in place while the boat moves away
//     }
// }

    public void ReceiveHealth()
    {
        if (damageTaken > 0)
        {
            damageTaken--;
        
            SetHealthSegmentTransparency(damageTaken, 1f); 
        }
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
    public TextMeshProUGUI highScoreText;
    public bool hasDied = false;
    public Animator animator;
    public Animator leaderboardAnimator;
    private float prevDistance = 0;

    public void SetDistance(float distance, float speed)
    {
        speedText.text = $"{speed:F0}m/s";

        if (distance > prevDistance + 0.1f) 
        {
            prevDistance = distance;

            float dividedDistance = distance / 1000;
            // Update the text to show distance rounded to 2 decimal places
            distanceText.text = $"{dividedDistance:F2} km";

            // Set the trigger on the animator to fire the animation
            animator.SetTrigger("TriggerDistanceGrow");
        }
    }

    public Slider slowMoSlider;           // Slider to represent slow motion duration
    public Animator sliderAnimator;       // Animator for the slider to control PlaySpeed
    public float refreshRate = 0.1f;      // Rate at which the slow-mo regenerates
    public float pauseDuration = 1.0f;    // Pause between deactivating and regenerating slow-mo

    private bool isRegenerating = false;  // Flag to indicate if regenerating
    private float regenTimer = 0f;        // Timer for the regeneration pause
    private bool isSpacePressed = false;  // Flag to track if space is pressed
    private float timeSinceSpaceRelease = 0f; // Timer for time since space was released

    void Update()
    {
        // If the space key is pressed and the slider has value left
        if (Input.GetKey(KeyCode.Space) && slowMoSlider.value > 0)
        {
            timeController.targetTimeScaleUI = 0.3f; // Set target game speed to 30%
            slowMoSlider.value -= Time.deltaTime / 2f; // Decrease slider value over time
            isRegenerating = false;
            regenTimer = 0f; // Reset the regeneration timer

            if (!isSpacePressed)
            {
                sliderAnimator.SetFloat("PlaySpeed", -10); // Set PlaySpeed to 1
                isSpacePressed = true;
            }
            timeSinceSpaceRelease = 0f;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            timeController.targetTimeScaleUI = 1.0f; // Set target game speed to 100%
        }
        else if (!hasDied)
        {
            if (isSpacePressed)
            {
                timeSinceSpaceRelease += Time.deltaTime;
                if (timeSinceSpaceRelease >= 1f)
                {
                    sliderAnimator.SetFloat("PlaySpeed", 1); // Set PlaySpeed to -1 after 1 second
                    isSpacePressed = false;
                }
            }

            // If slow motion has ended and not regenerating
            if (!isRegenerating)
            {
                regenTimer += Time.deltaTime;
                if (regenTimer >= pauseDuration)
                {
                    isRegenerating = true; // Start regenerating after the pause
                }
            }
            else
            {
                // Regenerate the slider value over time
                slowMoSlider.value += refreshRate * Time.deltaTime;
                slowMoSlider.value = Mathf.Clamp(slowMoSlider.value, 0, slowMoSlider.maxValue); // Clamp to max value
            }
        }

    }

    public void RemoveSlowMoValue(float amount, Rigidbody2D player, Vector2 newVelocity)
    {
        if (slowMoSlider.value >= amount)
        {
            slowMoSlider.value -= amount;
            slowMoSlider.value = Mathf.Clamp(slowMoSlider.value, 0, slowMoSlider.maxValue); // Clamp to max value

            player.velocity = newVelocity;
        }
        
        isRegenerating = false;
        regenTimer = 0f;
        sliderAnimator.SetFloat("PlaySpeed", -1); // Set PlaySpeed to 1
    }

    [System.Serializable]
    public class PostSaveEvent : UnityEvent { } // Serializable UnityEvent class

    public PostSaveEvent onHighScoreSaved; // Public UnityEvent field
    private IEnumerator SaveHighScoreAndSlowDown(float score)
    {
        float initialTimeScale = Time.timeScale;
        float targetTimeScale = 0.05f; // Target time scale for the slowdown
        float duration = 0.5f; // Duration of the slowdown
        float elapsedTime = 0f;

        // Gradually slow down the game over half a second
        GlobalLightController globalLightController = GameObject.FindAnyObjectByType<GlobalLightController>();
        globalLightController.LerpToColor(new Color(0.5f, 0.25f, 0.5f));
        while (elapsedTime < duration)
        {
            Time.timeScale = Mathf.Lerp(initialTimeScale, targetTimeScale, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = targetTimeScale;

        // Save the high score
        HighScoreManager.TrySaveHighScore(score, highScoreText);

        // Restore the time scale (optional if you want the game to resume)
        // Uncomment if you want to restore the time scale
        // elapsedTime = 0f;
        // while (elapsedTime < duration)
        // {
        //     Time.timeScale = Mathf.Lerp(targetTimeScale, initialTimeScale, elapsedTime / duration);
        //     elapsedTime += Time.unscaledDeltaTime;
        //     yield return null;
        // }
        // Time.timeScale = initialTimeScale;

        // Call the delegate if it's assigned
        onHighScoreSaved?.Invoke();
    }
}
