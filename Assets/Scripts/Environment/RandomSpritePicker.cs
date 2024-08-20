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

    public List<SpriteChance> spriteList; // List of sprites with their chances
    public List<SpriteReflection> reflections; // List of sprite reflections

    public GameObject reflectionPrefab;

    private SpriteRenderer spriteRenderer; // SpriteRenderer to assign the selected sprite
    private SpriteRenderer reflectionRenderer; // SpriteRenderer to assign the selected sprite

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer attached to the same GameObject

        if (spriteRenderer != null && spriteList.Count > 0)
        {
            Sprite selectedSprite = PickRandomSprite();
            spriteRenderer.sprite = selectedSprite;

            Sprite selectedReflection = PickReflectionSprite(selectedSprite);
            if (selectedReflection != null)
            {
                CreateReflection(selectedReflection);
            }
            else
            {
                Debug.LogWarning("No matching reflection found for the selected sprite.");
            }
        }
        else
        {
            Debug.LogWarning("SpriteRenderer is not attached or spriteList is empty.");
        }
    }

    void CreateReflection(Sprite reflectionSprite)
    {
        if (reflectionPrefab != null)
        {
            Vector3 reflectionLocation = new Vector3(transform.position.x, transform.position.y - 1.3f, transform.position.z);
            GameObject reflection = Instantiate(reflectionPrefab, reflectionLocation, Quaternion.identity);
            //reflection.transform.position.Set(reflection.transform.position.x, reflection.transform.position.y - 1, reflection.transform.position.z);

            SpriteRenderer reflectionRenderer = reflection.GetComponent<SpriteRenderer>();
            if (reflectionRenderer != null)
            {
                reflectionRenderer.sprite = reflectionSprite;
            }
            else
            {
                Debug.LogWarning("Reflection prefab does not have a SpriteRenderer component.");
            }
        }
        else
        {
            Debug.LogWarning("Reflection prefab is not assigned.");
        }
    }

    Sprite PickRandomSprite()
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

    Sprite PickReflectionSprite(Sprite selectedSprite)
    {
        foreach (var reflection in reflections)
        {
            if (reflection.sprite == selectedSprite)
            {
                return reflection.reflection;
            }
        }

        Debug.LogWarning("No matching reflection found for the selected sprite.");
        return null; // Return null if no matching reflection is found
    }
}
