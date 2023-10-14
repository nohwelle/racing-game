using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSprites : MonoBehaviour
{
    PlayerMovement playerMovement;
    SpriteRenderer playerSprite;

    public Sprite stand, crouch, slide;


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // standing
        if (!playerMovement.isCrouching && !playerMovement.isSliding)
        {
            playerSprite.sprite = stand;
        }

        // crouching
        if (playerMovement.isCrouching)
        {
            playerSprite.sprite = crouch;
        }
        
        // sliding
        if (playerMovement.isSliding)
        {
            playerSprite.sprite = slide;
        }
    }
}
