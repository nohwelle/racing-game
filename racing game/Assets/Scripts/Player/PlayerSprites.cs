using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSprites : MonoBehaviour
{
    Racer racer;
    CTFRunner ctfRunner;

    RacePlayerMovement racePlayerMovement;
    CTFPlayerMovement ctfPlayerMovement;

    RaceAIMovement raceAIMovement;
    CTFAIMovement ctfAIMovement;

    SpriteRenderer playerSprite;
    SpriteRenderer AISprite;

    public Sprite stand, crouch, slide, hurt;


    private void Awake()
    {
        racer = GetComponent<Racer>();
        ctfRunner = GetComponent<CTFRunner>();

        if (GetComponent<RacePlayer>())
        {
            racePlayerMovement = GetComponent<RacePlayerMovement>();
            playerSprite = GetComponent<SpriteRenderer>();
        }

        if (GetComponent<CTFPlayer>())
        {
            ctfPlayerMovement = GetComponent<CTFPlayerMovement>();
            playerSprite = GetComponent<SpriteRenderer>();
        }

        if (GetComponent<RaceAI>())
        {
            raceAIMovement = GetComponent<RaceAIMovement>();
            AISprite = GetComponent<SpriteRenderer>();
        }

        if (GetComponent<CTFAI>())
        {
            ctfAIMovement = GetComponent<CTFAIMovement>();
            AISprite = GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // standing
        if ((racePlayerMovement && !racePlayerMovement.isCrouching && !racePlayerMovement.isSliding) || (ctfPlayerMovement && !ctfPlayerMovement.isCrouching && !ctfPlayerMovement.isSliding))
        {
            playerSprite.sprite = stand;
        }
        if ((raceAIMovement && !raceAIMovement.isCrouching && !raceAIMovement.isSliding) || (ctfAIMovement && !ctfAIMovement.isCrouching && !ctfAIMovement.isSliding))
        {
            AISprite.sprite = stand;
        }

        // crouching
        if ((racePlayerMovement && racePlayerMovement.isCrouching) || (ctfPlayerMovement && ctfPlayerMovement.isCrouching))
        {
            playerSprite.sprite = crouch;
        }
        if ((raceAIMovement && raceAIMovement.isCrouching) || (ctfAIMovement && ctfAIMovement.isCrouching))
        {
            AISprite.sprite = crouch;
        }

        // sliding
        if ((racePlayerMovement && racePlayerMovement.isSliding) || (ctfPlayerMovement && ctfPlayerMovement.isSliding))
        {
            playerSprite.sprite = slide;
        }
        if ((raceAIMovement && raceAIMovement.isSliding) || (ctfAIMovement && ctfAIMovement.isSliding))
        {
            AISprite.sprite = slide;
        }
        
        // hurt
        if ((racer && racer.isInHitStun) || (ctfRunner && ctfRunner.isInHitStun))
        {
            if (playerSprite)
            {
                playerSprite.sprite = hurt;
            }

            if (AISprite)
            {
                AISprite.sprite = hurt;
            }
        }
    }
}
