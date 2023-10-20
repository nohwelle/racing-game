using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RaceCameraFollow : MonoBehaviour
{
    public Transform target;             // The target object
    public float boundaryY = 2f;         // The boundary in the Y-axis
    public float cameraAdjustment = 4f; // The amount to adjust the camera's Y position
    public float smoothingSpeed = 5f;   // The speed of the camera adjustment

    Vector3 lastPosition;

    private void Start()
    {
        lastPosition = new(0, transform.position.y - cameraAdjustment, -10);
    }

    void Update()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set.");
            return;
        }

        // Check if the target's Y position is beyond the boundary
        if (target.position.y > transform.position.y + boundaryY)
        {
            SetNewLastCameraPosition();
        }

        // Use Vector3.Lerp for smooth camera movement
        transform.position = Vector3.Lerp(transform.position, new Vector3(lastPosition.x, lastPosition.y + cameraAdjustment, lastPosition.z), smoothingSpeed * Time.deltaTime);
    }

    void SetNewLastCameraPosition()
    {
        lastPosition = transform.position;
    }
}
