using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StaticShadowSpawner : MonoBehaviour
{
    
    public Vector2 offset = new Vector2(0f, -0.5f);

    public void SpawnShadow(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found.");
            return;
        }

        // Create a new GameObject for the shadow
        GameObject shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(transform); // Set the parent to this GameObject
        shadowObject.transform.position = transform.position + (Vector3) offset; // Set position to (0,0,0) relative to the parent
        shadowObject.transform.localRotation = Quaternion.identity;

        // Add a SpriteRenderer component to the shadow object
        SpriteRenderer shadowSpriteRenderer = shadowObject.AddComponent<SpriteRenderer>();
        shadowSpriteRenderer.sprite = spriteRenderer.sprite; // Copy the sprite from the parent

        // Set the color to black with 0.4 alpha
        shadowSpriteRenderer.color = new Color(0f, 0f, 0f, 0.4f);

        shadowSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        shadowSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 10; // Subtract 10 from the parent's sorting order
    }
}
