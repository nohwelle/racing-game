using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIMovement : MonoBehaviour
{
    AI AI;

    public float intelligenceValue; // more intelligence value = more stupid
    public float moveSpeed = 16.25f;
    public float moveSpeedLimit = 3.25f;
    public float moveFriction = 0.1625f;
    public float jumpSpeed = 4.25f;
    public float slideSpeedFalloff = 0.005f;
    public Vector2 crouchSize;
    public Vector2 crouchOffset;
    public Vector2 slideSize;
    public Vector2 slideOffset;

    bool isFacingLeft;
    float horizontalInput;
    float lastHorizontalInput;
    public bool isGrounded;
    public bool isCrouching;
    public bool isSliding;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    BoxCollider2D playerCollider;
    Vector2 playerColliderStandingSize;
    Vector2 playerColliderStandingOffset;
    Vector2 playerColliderCrouchingSize;
    Vector2 playerColliderCrouchingOffset;
    Vector2 playerColliderSlidingSize;
    Vector2 playerColliderSlidingOffset;


    private void Awake()
    {
        AI = GetComponent<AI>();
        playerCollider = GetComponent<BoxCollider2D>();

        // set base intelligence
        intelligenceValue = Random.Range(0f, 0.5f);

        // set player collider sizes for standing & crouching
        playerColliderStandingSize = playerCollider.size;
        playerColliderStandingOffset = playerCollider.offset;

        playerColliderCrouchingSize = new Vector2(playerColliderStandingSize.x + crouchSize.x, playerColliderStandingSize.y + crouchSize.y);
        playerColliderCrouchingOffset = new Vector2(playerColliderStandingOffset.x + crouchOffset.x, playerColliderStandingOffset.y + crouchOffset.y);

        playerColliderSlidingSize = new Vector2(playerColliderStandingSize.x + slideSize.x, playerColliderStandingSize.y + slideSize.y);
        playerColliderSlidingOffset = new Vector2(playerColliderStandingOffset.x + slideOffset.x, playerColliderStandingOffset.y + slideOffset.y);
    }

    // Update is called once per frame
    void Update()
    {
        // get horizontal input with "smoothing"
        lastHorizontalInput = horizontalInput;
        horizontalInput += 0.01f;

        if (horizontalInput > 1 || horizontalInput < -1)
        {
            horizontalInput = Mathf.Sign(horizontalInput);
        }

        // check for ground collision
        GroundCheck();

        // check if player should turn around
        FlipDirection();

        // modify intelligence based on current position in relation to the player
        foreach (GameObject racer in GameManager.Instance.allRacers)
        {
            if (racer.GetComponent<Player>())
            {
                // get stupider when ahead of player -- more stupider if further ahead in general
                if (GetComponent<Racer>().currentPlacement > racer.GetComponent<Racer>().currentPlacement)
                {
                    intelligenceValue -= 0.001f * (GetComponent<Racer>().currentPlacement - racer.GetComponent<Racer>().currentPlacement);
                }

                // get unstupider when behind player -- less stupider if further behind in general
                if (GetComponent<Racer>().currentPlacement < racer.GetComponent<Racer>().currentPlacement)
                {
                    intelligenceValue += 0.001f * (racer.GetComponent<Racer>().currentPlacement - GetComponent<Racer>().currentPlacement);
                }
            }
        }

        if (intelligenceValue > 0.5f)
        {
            intelligenceValue = 0.5f;
        }
        if (intelligenceValue < 0)
        {
            intelligenceValue = 0;
        }
    }

    private void FixedUpdate()
    {
        // mooooove!
        if (!isSliding && !isCrouching && horizontalInput != 0)
        {
            rb.velocity += new Vector2(horizontalInput * moveSpeed * Time.deltaTime, 0);
        }
        if (isCrouching)
        {
            rb.velocity += new Vector2(horizontalInput * moveSpeed / 2 * Time.deltaTime, 0);
        }

        // slow player down if not inputting anything
        if (Mathf.Abs(horizontalInput) <= Mathf.Abs(lastHorizontalInput) && Mathf.Abs(horizontalInput) < 1 && rb.velocity.x != 0)
        {
            if (!isSliding)
            {
                rb.velocity -= new Vector2(moveFriction * Mathf.Sign(rb.velocity.x), 0);
            }

            // stop movement sooner by setting velocity to 0 after it gets lower than moveFriction
            if (rb.velocity.x <= moveFriction && rb.velocity.x >= -moveFriction)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        // limit speed
        if (rb.velocity.x > moveSpeedLimit || rb.velocity.x < -moveSpeedLimit)
        {
            rb.velocity = new Vector2(moveSpeedLimit * Mathf.Sign(rb.velocity.x), rb.velocity.y);
        }
        if (isCrouching && (rb.velocity.x > moveSpeedLimit / 2 || rb.velocity.x < -moveSpeedLimit / 2))
        {
            rb.velocity = new Vector2(moveSpeedLimit / 2 * Mathf.Sign(rb.velocity.x), rb.velocity.y);
        }
    }

    void FlipDirection()
    {
        if (isFacingLeft && rb.velocity.x > 0 || !isFacingLeft && rb.velocity.x < 0)
        {
            isFacingLeft = !isFacingLeft;
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }

    void GroundCheck()
    {
        // detect if a groundLayer object is within the 0.1 unit gap between the player's collider size and groundCheck's position
        if (Physics2D.OverlapBox(groundCheck.position, playerCollider.size, 0, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>())
        {
            // if obstacle is above player, crouch/slide under it
            if (collision.gameObject.transform.position.y - transform.position.y > 0)
            {
                if (rb.velocity.x != 0)
                {
                    StartCoroutine(Think(StartSliding()));
                }
                else if (rb.velocity.x == 0)
                {
                    StartCoroutine(Think(StartCrouching()));
                }
            }

            // if obstacle is below player, jump over it
            if (collision.gameObject.transform.position.y - transform.position.y < 0)
            {
                StartCoroutine(Think(Jump()));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.transform.position.y - transform.position.y > 0)
        {
            if (rb.velocity.x == 0)
            {
                StartCoroutine(Think(StartCrouching()));
            }
        }

        if (collision.gameObject.GetComponent<Obstacle>())
        {
            if (isSliding && rb.velocity.x == 0)
            {
                EndSliding();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Obstacle>())
        {
            if (isSliding)
            {
                EndSliding();
            }

            if (isCrouching)
            {
                EndCrouching();
            }
        }
    }

    IEnumerator Think(IEnumerator coroutineName)
    {
        // randomly decide to either stop in place or just keep moving
        if (Random.Range(0, 1) == 1)
        {

        }

        // enforce delay on actions before executing
        yield return new WaitForSeconds(Random.Range(intelligenceValue, intelligenceValue + 0.25f));

        StartCoroutine(coroutineName);

        yield break;
    }

    IEnumerator Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

        yield break;
    }

    IEnumerator StartCrouching()
    {
        isCrouching = true;
        playerCollider.size = playerColliderCrouchingSize;
        playerCollider.offset = playerColliderCrouchingOffset;

        yield return null;
    }

    void EndCrouching()
    {
        isCrouching = false;
        playerCollider.size = playerColliderStandingSize;
        playerCollider.offset = playerColliderStandingOffset;
    }

    IEnumerator StartSliding()
    {
        isSliding = true;
        horizontalInput = 0;

        rb.velocity -= new Vector2(slideSpeedFalloff * Mathf.Sign(rb.velocity.x), 0);

        playerCollider.size = playerColliderSlidingSize;
        playerCollider.offset = playerColliderSlidingOffset;

        yield return null;
    }

    void EndSliding()
    {
        isSliding = false;
        if (!isCrouching)
        {
            playerCollider.size = playerColliderStandingSize;
            playerCollider.offset = playerColliderStandingOffset;
        }
    }
}
