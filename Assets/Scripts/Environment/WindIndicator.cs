using UnityEngine;

public class WindIndicator : MonoBehaviour
{
    public ParticleSystem windParticleSystem;  // Reference to the Particle System
    public Rigidbody2D playerRb;               // Reference to the player's Rigidbody2D
    public float windStrength = 1f;            // Multiplier for the wind strength

    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.TrailModule trailModule;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        if (windParticleSystem == null)
        {
            Debug.LogError("Particle System is not assigned.");
            return;
        }

        // Configure particle system
        mainModule = windParticleSystem.main;
        trailModule = windParticleSystem.trails;
        particles = new ParticleSystem.Particle[windParticleSystem.main.maxParticles];

        // Make sure the particle system is attached to the camera
        windParticleSystem.transform.SetParent(Camera.main.transform);
        windParticleSystem.transform.localPosition = Vector3.zero;

        // Set the particle system to continuously emit particles
        mainModule.loop = true;
        mainModule.playOnAwake = true;
        mainModule.duration = Mathf.Infinity;

        // Configure the particle system to emit small spheres with trails
        mainModule.startSize = 0.1f; // Adjust size of particles
        mainModule.startLifetime = 2f; // Adjust lifetime of particles
        mainModule.gravityModifier = 0f; // Disable default gravity
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        // Configure trails
        trailModule.enabled = true;
        // trailModule.startColor = Color.white; // Adjust color as needed
        // trailModule.startWidth = 0.1f; // Adjust width as needed

        // Prepare the particle array for manipulation
        particles = new ParticleSystem.Particle[windParticleSystem.main.maxParticles];
    }

    void Update()
    {
        if (playerRb != null && windParticleSystem != null)
        {
            // Get the velocity of the player
            Vector2 playerVelocity = playerRb.velocity;
            Vector3 windDirection = -new Vector3(playerVelocity.x, playerVelocity.y, 0f).normalized;

            // Adjust gravity based on player's velocity
            Vector3 gravity = windDirection * windStrength;
            mainModule.gravityModifier = 0f; // Disable default gravity
            windParticleSystem.gravityModifier = gravity.magnitude;

            // Emit particles with direction opposite to the player's movement
            EmitWindParticles(windDirection);

            // Update particle system to follow the camera
            windParticleSystem.transform.position = Camera.main.transform.position;

            // Update particle system rotation to match the direction opposite to the player's movement
            float angle = Mathf.Atan2(-playerVelocity.y, -playerVelocity.x) * Mathf.Rad2Deg;
            windParticleSystem.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void EmitWindParticles(Vector3 direction)
    {
        // Emit a number of particles
        int particleCount = 10; // Number of particles to emit
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
        {
            velocity = direction * 10f // Set initial velocity to be opposite to player’s movement
        };

        windParticleSystem.Emit(emitParams, particleCount);
    }
}
