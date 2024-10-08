using System;
using UnityEngine;

public class SwingingController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float swingForce = 10f;
    private bool isSwinging = false;
    public float pushForce = 25f;
    private bool isPulling = false;
    private Vector2 swingDirection;
    private Vector2 attachPoint;
    private Rigidbody2D rb;
    private Attachable currentAttachable; // Reference to the current attachable

    private GameUI gameUI;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned.");
        }
        gameUI = GameObject.FindObjectOfType<GameUI>();
    }

    void Update()
    {
        if (gameUI.hasDied)
        {
            return;
        }
        
        if (gameUI != null)
        {
            gameUI.SetDistance(transform.position.y, rb.velocity.magnitude);
        }

        if (Input.GetMouseButton(0) && !isSwinging)
        {
            FindClosestAttachable();
        }

        if (Input.GetMouseButton(0) && isSwinging)
        {
            Swing();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopSwinging();
        }

        if (Input.GetMouseButtonDown(1))
        {
            ApplyPushForce();
        }

        isPulling = Input.GetMouseButton(1);
    }

    [Range(0f, 1f)] public float forceStealPercentage = 0.5f;
    [Range(0f, 1f)] public float forcePrice = 0.17f;

    void ApplyPushForce()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePosition - (Vector2)transform.position).normalized;

        Vector2 originalVelocity = rb.velocity;

        Vector2 newVelocity = originalVelocity * (1f - forceStealPercentage) + forceStealPercentage * originalVelocity.magnitude * directionToMouse;

        gameUI.RemoveSlowMoValue(forcePrice, rb, newVelocity);
    }

    void FindClosestAttachable()
    {
        GameObject[] attachables = GameObject.FindGameObjectsWithTag("Attachable");
        GameObject closestAttachable = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject attachable in attachables)
        {
            float distance = Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), attachable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAttachable = attachable;
            }
        }
        
        if (closestAttachable != null)
        {
            attachPoint = closestAttachable.transform.position;

            closestAttachable.TryGetComponent<Attachable>(out currentAttachable);

            if (currentAttachable != null)
            {
                currentAttachable.OnConnect(transform);
            }

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, attachPoint);
            isSwinging = true;
        }
    }

    void Swing()
    {
        Vector2 playerPosition = (Vector2)transform.position;
        Vector2 directionToAttachPoint = (attachPoint - playerPosition).normalized;
        Vector2 velocity = rb.velocity;

        // Calculate the perpendicular direction to the swing
        Vector2 perpendicularDirection = new Vector2(-directionToAttachPoint.y, directionToAttachPoint.x);

        // Calculate the current tangential velocity component
        float tangentialSpeed = Vector2.Dot(velocity, perpendicularDirection);

        // Determine the swing direction based on the tangential speed
        swingDirection = tangentialSpeed > 0 ? perpendicularDirection : -perpendicularDirection;

        // Update the player's velocity to maintain the current tangential speed
        rb.velocity = swingDirection * Mathf.Abs(tangentialSpeed);

        if (isPulling)
        {
            float pullStrength = 25f; // Adjust the strength of the pull force
            Vector2 pullForce = directionToAttachPoint * pullStrength;
        }
        else 
        {
            rb.velocity *= 1.001f;
        }

        // Update the line renderer positions
        lineRenderer.SetPosition(0, playerPosition);
        lineRenderer.SetPosition(1, attachPoint);
    }

    void StopSwinging()
    {
        if (currentAttachable != null)
        {
            currentAttachable.OnDisconnect(); // Call OnDisconnect on the current attachable
        }

        isSwinging = false;
        lineRenderer.positionCount = 0;
        currentAttachable = null; // Clear the reference to the current attachable
    }
}
