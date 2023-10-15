using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public KeyCode resetKey;
    public TMP_Text roomsCompletedText;

    public float roomsCompleted;

    Vector2 spawnPoint;

    private void Start()
    {
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (resetKey != KeyCode.None && Input.GetKeyDown(resetKey))
        {
            transform.position = spawnPoint;
        }

        // update rooms completed text
        if (roomsCompletedText && roomsCompletedText.text != (roomsCompleted + 1).ToString())
        {
            if (roomsCompleted + 1 < 10)
            {
                roomsCompletedText.text = "ROOM 0" + (roomsCompleted + 1).ToString();
            }
            else
            {
                roomsCompletedText.text = "ROOM " + (roomsCompleted + 1).ToString();
            }
        }
    }
}
