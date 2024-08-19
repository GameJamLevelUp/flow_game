using UnityEngine;

public class ShadowImageFollower : MonoBehaviour
{
    public Transform followTransform;
    public Vector3 worldOffset = new Vector3(0, 0.1f, 0);

    void Update()
    {
        if (followTransform != null)
        {
            // Convert the local offset to world space
            Vector3 worldOffsetPosition = followTransform.TransformPoint(worldOffset);

            // Calculate the world position with the desired offset
            Vector3 worldPosition = followTransform.position + (worldOffsetPosition - followTransform.position);

            // Update position and rotation
            transform.position = worldPosition;
            transform.rotation = followTransform.rotation;
        }
    }
}
