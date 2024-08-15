using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToMoveDirection : MonoBehaviour
{
    public Rigidbody2D rb; // Reference to the Rigidbody
    public float rotationOffset = 0f; // Rotation offset in degrees

    void Update()
    {
        if (rb != null)
        {
            // Get the velocity of the Rigidbody
            Vector3 velocity = rb.velocity;

            if (velocity != Vector3.zero)
            {
                // Calculate the direction of travel
                Vector3 direction = velocity.normalized;

                // Calculate the rotation to align with the direction
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Apply the rotation offset
                targetRotation *= Quaternion.Euler(0, rotationOffset, 0);

                // Set the transform's rotation
                transform.rotation = targetRotation;
            }
        }
    }
}
