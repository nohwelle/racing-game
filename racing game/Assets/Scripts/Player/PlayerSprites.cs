using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSprites : MonoBehaviour
{
    PlayerMovement playerMovement;
    AIMovement AIMovement;
    SpriteRenderer playerSprite;
    SpriteRenderer AISprite;

    public Sprite stand, crouch, slide;


    private void Awake()
    {
        if (GetComponent<Player>())
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerSprite = GetComponent<SpriteRenderer>();
        }

        if (GetComponent<AI>())
        {
            AIMovement = GetComponent<AIMovement>();
            AISprite = GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // standing
        if (playerMovement && !playerMovement.isCrouching && !playerMovement.isSliding)
        {
            playerSprite.sprite = stand;
        }
        if (AIMovement && !AIMovement.isCrouching && !AIMovement.isSliding)
        {
            AISprite.sprite = stand;
        }

        // crouching
        if (playerMovement && playerMovement.isCrouching)
        {
            playerSprite.sprite = crouch;
        }
        if (AIMovement && AIMovement.isCrouching)
        {
            AISprite.sprite = crouch;
        }

        // sliding
        if (playerMovement && playerMovement.isSliding)
        {
            playerSprite.sprite = slide;
        }
        if (AIMovement && AIMovement.isSliding)
        {
            AISprite.sprite = slide;
        }
    }
}
