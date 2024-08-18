using System;
using UnityEngine;

public class SwingingController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float swingForce = 10f;
    public float retractingForceStrength = 1000f;
    private bool isSwinging = false;
    private Vector2 attachPoint;
    private float ropeLength;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        {
            Debug.LogError("LineRenderer is not assigned.");
        }
    }

    void Update()
    {

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
    }

    void FindClosestAttachable()
    {
        GameObject[] attachables = GameObject.FindGameObjectsWithTag("Attachable");
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
        
        print(closestAttachable);
        if (closestAttachable != null)
        {
            attachPoint = closestAttachable.transform.position;
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

        // Update the player's velocity to maintain the current tangential speed
        float distance = Mathf.Clamp(Vector2.Distance(playerPosition, attachPoint), 1, float.MaxValue);
        rb.AddForce(directionToAttachPoint * retractingForceStrength * Time.deltaTime);

        // Update the line renderer positions
        lineRenderer.SetPosition(0, playerPosition);
        lineRenderer.SetPosition(1, attachPoint);

        // Calculate the ratio of the used rope length
        float ropeRatio = Mathf.Clamp(distance / ropeLength, 0.5f, 1);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0.0f),
                new GradientColorKey(Color.white, 0.8f),
                new GradientColorKey(Color.red, 1.0f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.0f, 0.0f),
                new GradientAlphaKey(0.8f, 0.8f),
                new GradientAlphaKey(0.8f, 1.0f)
            }
        );

        // Set the color of the line renderer based on the rope ratio
        lineRenderer.colorGradient = gradient;
        lineRenderer.startColor = gradient.Evaluate(ropeRatio);
        lineRenderer.endColor = gradient.Evaluate(ropeRatio);
    }


    void StopSwinging()
    {
        isSwinging = false;
        lineRenderer.positionCount = 0;

        if (closestAttachable != null)
        {
            Destroy(joint);
            
        }
        
        //rb.velocity = Vector2.zero;
    }
}
