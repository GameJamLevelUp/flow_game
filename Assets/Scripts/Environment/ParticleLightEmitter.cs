using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticleLightEmitter : MonoBehaviour
{
    public GameObject lightPrefab; // The prefab of the 2D light to attach to particles
    new public ParticleSystem particleSystem; // The particle system to which the lights will be attached

    private ParticleSystem.Particle[] particles; // Array to hold particles
    private Light2D[] lights; // Array to hold the lights attached to the particles

    void Start()
    {
        // Initialize the particle and light arrays based on the maximum number of particles
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        lights = new Light2D[particleSystem.main.maxParticles];

        // Instantiate the light prefabs but deactivate them initially
        for (int i = 0; i < lights.Length; i++)
        {
            GameObject lightInstance = Instantiate(lightPrefab, transform);
            lights[i] = lightInstance.GetComponent<Light2D>();
            lights[i].enabled = false; // Initially disable the light
        }
    }

    void Update()
    {
        // Get the number of active particles in the system
        int numParticlesAlive = particleSystem.GetParticles(particles);

        // Loop through each particle and position the corresponding light at the particle's world position
        for (int i = 0; i < lights.Length; i++)
        {
            if (i < numParticlesAlive)
            {
                // Convert particle position from local to world space
                Vector3 worldPosition = particleSystem.transform.TransformPoint(particles[i].position);

                // Position the light at the particle's world position
                lights[i].transform.position = worldPosition;
                lights[i].enabled = true; // Enable the light
            }
            else
            {
                // Disable any lights that are not needed
                lights[i].enabled = false;
            }
        }
    }
}
