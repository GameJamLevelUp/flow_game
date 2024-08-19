using System;
using UnityEngine;

public class SwingingController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float swingForce = 10f;
    private bool isSwinging = false;
    private bool isPulling = false;
    private Vector2 swingDirection;
    private Vector2 attachPoint;
    private Rigidbody2D rb;
    
    private GameUI gameUI;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        {
            Debug.LogError("LineRenderer is not assigned.");
        }
        gameUI = GameObject.FindObjectOfType<GameUI>();
    }

    void Update()
    {

        if (gameUI != null)
        {
            gameUI.SetDistance(transform.position.y, rb.velocity.magnitude);
        }

        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !isSwinging)
        {
            FindClosestAttachable();
        }

        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && isSwinging)
        {
            Swing();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            StopSwinging();
        }

        isPulling = Input.GetMouseButton(1);
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
        
        Vector3 movementDirection = rb.velocity.normalized;

        // Calculate the direction towards the attachable point
        Vector3 directionToAttachablePoint = ((Vector2)closestAttachable.transform.position - rb.position).normalized;

        // Calculate the angle between the two directions
        float angle = Vector3.Angle(movementDirection, directionToAttachablePoint);

        // Check if the angle is between 80 and 100 degrees
        // if (angle >= 80f && angle <= 100f)
        // {


            if (closestAttachable != null)
            {
                attachPoint = closestAttachable.transform.position;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, attachPoint);
                isSwinging = true;
            }
        // }
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
            rb.AddForce(pullForce, ForceMode2D.Force);
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
        isSwinging = false;
        lineRenderer.positionCount = 0;
        //rb.velocity = Vector2.zero;
    }
}