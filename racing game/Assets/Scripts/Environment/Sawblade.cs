using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawblade : MonoBehaviour
{
    public float rotationSpeed = 0.5f;

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed));
    }
}
