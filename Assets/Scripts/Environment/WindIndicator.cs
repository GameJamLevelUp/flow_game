using UnityEngine;

public class AlignParticleRotation : MonoBehaviour
{
    public Rigidbody2D rb;                // The Rigidbody2D to track
    public ParticleSystem particleSystem; // The ParticleSystem whose rotation and emission rate will be updated

    public float emissionMultiplier = 1.0f; // Multiplier to scale emission rate based on speed

    private ParticleSystem.EmissionModule emissionModule;

    private void Start()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is not assigned!");
            return;
        }

        if (particleSystem == null)
        {
            Debug.LogError("ParticleSystem is not assigned!");
            return;
        }

        // Get the emission module from the ParticleSystem
        emissionModule = particleSystem.emission;
    }

    private void Update()
    {
        if (rb == null || particleSystem == null)
        {
            return;
        }

        // Get the velocity of the Rigidbody2D
        Vector2 velocity = rb.velocity;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;

        // Set the rotation of the particle system's transform
        particleSystem.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Adjust the emission rate based on the velocity
        float speed = velocity.magnitude;
        emissionModule.rateOverTime = speed * emissionMultiplier;
    }
}
