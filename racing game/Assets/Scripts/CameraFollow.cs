using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The object you want the camera to follow
    public float followSpeed = 5.0f; // Speed at which the camera follows the target
    public float followThreshold = 1.0f; // Distance from the target to trigger camera movement

    private float initialY; // Initial camera Y position relative to the target

    void Start()
    {
        if (target != null)
        {
            initialY = transform.position.y - target.position.y;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Calculate the desired Y position of the camera
        float desiredY = target.position.y + initialY;

        // Check if the target has moved outside the follow threshold
        if (Mathf.Abs(transform.position.y - desiredY) > followThreshold)
        {
            // Smoothly move the camera towards the desired Y position
            float newY = Mathf.Lerp(transform.position.y, desiredY, followSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}
