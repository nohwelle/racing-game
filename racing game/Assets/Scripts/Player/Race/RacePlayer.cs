using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RacePlayer : MonoBehaviour
{
    public KeyCode resetKey;
    public TMP_Text roomsCompletedText;
    public TMP_Text currentPlacementText;

    public Material playerMaterial;
    public Color playerColor = new(1f, 1f, 1f, 0f);
    public Color playerOutlineColor = new(0f, 0f, 0f, 0f);

    Racer racer;

    private void Awake()
    {
        racer = GetComponent<Racer>();

        playerMaterial = GetComponent<SpriteRenderer>().material;
        playerMaterial.SetColor("_RacePlayer_Color", playerColor);
        playerMaterial.SetColor("_Outline_Color", playerOutlineColor);
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG -- reset player to spawn point
        if (resetKey != KeyCode.None && Input.GetKeyDown(resetKey))
        {
            transform.position = RaceGameManager.Instance.spawnPoint.transform.position;
        }

        // update rooms completed text
        if (roomsCompletedText)
        {
            if (racer.roomsCompleted + 1 < 10)
            {
                roomsCompletedText.text = "ROOM 0" + (racer.roomsCompleted + 1).ToString();
            }
            else
            {
                roomsCompletedText.text = "ROOM " + (racer.roomsCompleted + 1).ToString();
            }
        }

        // update current placement text
        // -- FIGURE OUT HOW TO APPEND CORRECT SUFFIX
        if (currentPlacementText)
        {
            currentPlacementText.text = racer.currentPlacement.ToString() + "/" + RaceGameManager.Instance.allRacers.Count.ToString();
        }
    }
}
