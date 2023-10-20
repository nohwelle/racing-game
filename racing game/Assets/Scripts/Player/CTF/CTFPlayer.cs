using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CTFPlayer : MonoBehaviour
{
    public KeyCode resetKey;

    public Material playerMaterial;
    public Color playerColor = new(1f, 1f, 1f, 0f);
    public Color playerOutlineColor = new(0f, 0f, 0f, 0f);

    private void Awake()
    {
        playerMaterial = GetComponent<SpriteRenderer>().material;
        playerMaterial.SetColor("_Player_Color", playerColor);
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
    }
}
