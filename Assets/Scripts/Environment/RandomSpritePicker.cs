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

    public List<SpriteChance> spriteList; // List of sprites with their chances

    private SpriteRenderer spriteRenderer; // SpriteRenderer to assign the selected sprite

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer attached to the same GameObject

        if (spriteRenderer != null && spriteList.Count > 0)
        {
            Sprite selectedSprite = PickRandomSprite();
            spriteRenderer.sprite = selectedSprite;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer is not attached or spriteList is empty.");
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
}
