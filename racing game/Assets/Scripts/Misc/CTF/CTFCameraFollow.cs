using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CTFCameraFollow : MonoBehaviour
{
    public Transform target;             // The target object
    public float smoothingSpeed = 5f;   // The speed of the camera adjustment

    Vector3 lastPosition;

    private void Start()
    {
        SetNewLastCameraPosition();
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set.");
            return;
        }

        SetNewLastCameraPosition();
    }

    void SetNewLastCameraPosition()
    {
        lastPosition = new(target.transform.position.x, target.transform.position.y, transform.position.z);

        // Use Vector3.Lerp for smooth camera movement
        transform.position = Vector3.Lerp(transform.position, lastPosition, smoothingSpeed * Time.deltaTime);
    }
}
