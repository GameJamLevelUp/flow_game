using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowImageFollower : MonoBehaviour
{
    public Transform followTransform;
    public Vector3 worldOffset = new Vector3(0, 0.1f, 0);

    void Update()
    {
        if (followTransform != null)
        {
            // Calculate the world position with the desired offset
            Vector3 worldPosition = followTransform.position + followTransform.TransformVector(worldOffset);
            transform.position = worldPosition;
            transform.rotation = followTransform.rotation;
        }
    }
}
