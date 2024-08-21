using UnityEngine;

public class ConnectObjectsWithSprite : MonoBehaviour
{
    public Transform object1;  // Reference to the first object
    public Transform object2;  // Reference to the second object
    private SpriteRenderer spriteRenderer;  // Reference to the sprite renderer

    private int frameCount = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Wait for 5 frames before adjusting the sprite
        if (frameCount < 5)
        {
            frameCount++;
            return;
        }

        // Calculate the distance between the two objects
        float distance = Vector2.Distance(object1.position, object2.position);

        // Calculate the sprite's width in world units
        float spriteWidthInWorldUnits = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit;

        // Set the sprite's scale to match the distance (adjusting for the sprite's width)
        transform.localScale = new Vector3(distance / spriteWidthInWorldUnits, distance / spriteWidthInWorldUnits, transform.localScale.z);

        // Calculate the midpoint between the two objects
        Vector3 midpoint = (object1.position + object2.position) / 2f;

        // Set the position of the sprite to the midpoint
        transform.position = midpoint;

        // Calculate the angle between the two objects and rotate the sprite accordingly
        Vector2 direction = object2.position - object1.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // No further updates are needed, so disable the script
        enabled = false;
    }
}
