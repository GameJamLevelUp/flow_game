using System.Collections.Generic;
using UnityEngine;

public class RandomSpritePicker : MonoBehaviour
{
    [System.Serializable]
    public class SpriteChance
    {
        public Sprite sprite; // The sprite to pick
        public float chance; // The chance of picking this sprite (0 to 1)
    }

    [System.Serializable]
    public class SpriteReflection
    {
        public Sprite sprite;
        public Sprite reflection;
    }

    public Material reflectionMaterial;

    public List<SpriteChance> spriteList; // List of sprites with their chances
    public List<SpriteReflection> reflections; // List of sprite reflections

    public GameObject reflectionPrefab;

    private SpriteRenderer spriteRenderer; // SpriteRenderer to assign the selected sprite
    public Vector2 offset = new Vector2(0f, -1f); // Offset to place the reflection below the object
    public float reflectionAlpha = 0.6f; // Opacity of the reflection
    public bool cumulative = true;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the sprite renderer of the current object

        if (spriteRenderer != null && spriteList.Count > 0)
        {
            Sprite selectedSprite = PickRandomSprite();
            if (selectedSprite == null)
            {
                Destroy(gameObject);
                return;
            }
            spriteRenderer.sprite = selectedSprite;

            SpawnReflection(spriteRenderer);
        }
        else
        {
            Debug.LogWarning("SpriteRenderer is not attached to the object.");
        }
    }

    public void SpawnReflection(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found.");
            return;
        }

        // Create a new GameObject for the reflection
        GameObject reflectionObject = new GameObject("Reflection");
        reflectionObject.transform.SetParent(transform); // Set the parent to this GameObject
        reflectionObject.transform.position = transform.position + (Vector3)offset; // Set position based on offset
        reflectionObject.transform.localRotation = Quaternion.identity;

        // Add a SpriteRenderer component to the reflection object
        SpriteRenderer reflectionSpriteRenderer = reflectionObject.AddComponent<SpriteRenderer>();
        reflectionSpriteRenderer.sprite = spriteRenderer.sprite; // Copy the sprite from the parent

        if (reflectionMaterial != null)
        {
            reflectionSpriteRenderer.material = reflectionMaterial;
            reflectionMaterial.SetFloat("_Distortion", 0.9f); // Adjust as needed
        }

        // Set the reflection color with the specified alpha
        reflectionSpriteRenderer.color = new Color(1f, 1f, 1f, reflectionAlpha);

        // Flip the reflection vertically to mimic water reflection
        reflectionSpriteRenderer.flipY = true;

        // Set sorting layer and order to render the reflection correctly
        reflectionSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        reflectionSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1; // Adjust sorting order if needed
    }

    Sprite PickRandomSprite()
    {
        if (cumulative)
        {
            float totalChance = 0f;
            foreach (var item in spriteList)
            {
                totalChance += item.chance;
            }

            float randomValue = Random.value * totalChance;
            float cumulativeChance = 0f;

            foreach (var item in spriteList)
            {
                cumulativeChance += item.chance;
                if (randomValue <= cumulativeChance)
                {
                    return item.sprite;
                }
            }

            // Return the last sprite in the list as a fallback
            return spriteList[spriteList.Count - 1].sprite;
        }
        else 
        {
            float randomValue = Random.value;

            foreach (var item in spriteList)
            {
                
                if (randomValue <= item.chance)
                {
                    return item.sprite;
                }
            }
            return null;
        }
        
    }
}
