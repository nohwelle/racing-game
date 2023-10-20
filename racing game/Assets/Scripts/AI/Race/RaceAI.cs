using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RaceAI : MonoBehaviour
{
    public Material playerMaterial;
    public Color playerColor = new(1f, 1f, 1f, 0f);
    public Color playerOutlineColor = new(0f, 0f, 0f, 0f);

    float currentPosition;

    Racer racer;

    private void Awake()
    {
        racer = GetComponent<Racer>();

        // set random color for AI
        playerMaterial = GetComponent<SpriteRenderer>().material;

        playerColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
        playerOutlineColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);

        playerMaterial.SetColor("_RacePlayer_Color", playerColor);
        playerMaterial.SetColor("_Outline_Color", playerOutlineColor);
    }
}
