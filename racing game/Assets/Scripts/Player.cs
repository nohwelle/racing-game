using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public KeyCode resetKey;

    public float roomsCompleted;

    Vector2 spawnPoint;

    private void Start()
    {
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            transform.position = spawnPoint;
        }
    }
}
