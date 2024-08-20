using UnityEngine;
using UnityEngine.UI;

public abstract class Attachable : MonoBehaviour
{
    public abstract void OnComplete();

    public Canvas canvas;
    public Image radialPanel; // Radial panel to visualize rotation

    private Vector3 initialPlayerPosition; // To store the initial position of the player
    private Transform playerTransform;
    private bool isClockwise; // Determine fill direction

    public void OnConnect(Transform pT)
    {
        playerTransform = pT;
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            if (canvasRectTransform != null)
            {
                cumulativeAngle = 0f;
                previousAngle = 0f;
                // Set the canvas position to the attachable's position
                canvasRectTransform.position = transform.position;

                // Rotate the canvas to face the player direction in 2D space
                Vector3 toDirection = (playerTransform.position - transform.position).normalized;
                float angle = Mathf.Atan2(toDirection.y, toDirection.x) * Mathf.Rad2Deg;
                canvasRectTransform.rotation = Quaternion.Euler(0, 0, angle);

                // Store the initial player position for later use
                initialPlayerPosition = playerTransform.position;
            }
            else
            {
                Debug.LogError("The canvas does not have a RectTransform component.");
            }
        }
        else
        {
            Debug.LogError("Canvas is not assigned.");
        }
    }

    public void OnDisconnect()
    {
        canvas.gameObject.SetActive(false);
    }

 private float previousAngle = 0f;
private float cumulativeAngle = 0f;

private void Update()
{
    if (radialPanel != null && playerTransform != null)
    {
        // Calculate the angle between the initial and current player positions in 2D space
        Vector3 currentPlayerPosition = playerTransform.position;

        Vector2 initialDirection = initialPlayerPosition - transform.position;
        Vector2 currentDirection = currentPlayerPosition - transform.position;

        // Calculate the angle between the initial and current directions
        float currentAngle = Vector2.SignedAngle(initialDirection, currentDirection);

        // Determine the angle difference since the last update
        float angleDelta = Mathf.DeltaAngle(previousAngle, currentAngle);

        // Update the cumulative angle moved
        cumulativeAngle += angleDelta;

        // Update the radial panel fill amount
        if (cumulativeAngle >= 0)
        {
            radialPanel.fillClockwise = false;
            radialPanel.fillAmount = Mathf.Clamp01(cumulativeAngle / 360f);
        }
        else
        {
            radialPanel.fillClockwise = true;
            radialPanel.fillAmount = Mathf.Clamp01(-cumulativeAngle / 360f);
        }

        // Store the current angle as the previous angle for the next update
        previousAngle = currentAngle;

        if (Mathf.Abs(cumulativeAngle) > 360)
        {
            OnComplete();
            OnDisconnect();
            cumulativeAngle = 0f;
            previousAngle = 0f;
        }
    }
}

}
