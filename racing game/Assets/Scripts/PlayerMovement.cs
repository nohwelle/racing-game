using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public KeyCode jumpKey;
    public KeyCode crouchKey;

    public float moveSpeed = 3.25f;
    public float jumpSpeed = 4.25f;
    public float slideSpeedFalloff = 0.005f;
    public Vector2 crouchSize;
    public Vector2 crouchOffset;
    public Vector2 slideSize;
    public Vector2 slideOffset;

    bool isFacingLeft;
    float horizontalInput;
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
        playerCollider = GetComponent<BoxCollider2D>();

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
        // get horizontal input with smoothing
        horizontalInput = Input.GetAxis("Horizontal");

        // check for ground collision
        GroundCheck();

        // perform jump
        if (Input.GetKeyDown(jumpKey) && isGrounded && (!isSliding || !isCrouching))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        // start or end crouch/slide
        if (Input.GetKey(crouchKey) && isGrounded && rb.velocity.x == 0)
        {
            StartCrouching();
        }
        if (Input.GetKeyUp(crouchKey) && isGrounded)
        {
            EndCrouching();
        }
        if (Input.GetKey(crouchKey) && isGrounded && rb.velocity.x != 0)
        {
            StartSliding();
        }
        if (Input.GetKeyUp(crouchKey) && isGrounded)
        {
            EndSliding();
        }

        // check if player should turn around
        FlipDirection();
    }

    private void FixedUpdate()
    {
        if (!isSliding && !isCrouching)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
        if (isCrouching)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed / 2, rb.velocity.y);
        }
    }

    void FlipDirection()
    {
        if (isFacingLeft && horizontalInput > 0 || !isFacingLeft && horizontalInput < 0)
        {
            isFacingLeft = !isFacingLeft;
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }

    void GroundCheck()
    {
        // detect if a groundLayer object is within the 0.1 unit gap between the player's localScale and groundCheck's position
        if (Physics2D.OverlapBox(groundCheck.position, transform.localScale, 0, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void StartCrouching()
    {
        isCrouching = true;
        playerCollider.size = playerColliderCrouchingSize;
        playerCollider.offset = playerColliderCrouchingOffset;
    }

    void EndCrouching()
    {
        isCrouching = false;
        playerCollider.size = playerColliderStandingSize;
        playerCollider.offset = playerColliderStandingOffset;
    }

    void StartSliding()
    {
        isSliding = true;
        float lastXVelocity = rb.velocity.x;
        float lastXDirection = Mathf.Sign(rb.velocity.x);
        horizontalInput = 0;

        if (Mathf.Sign(lastXVelocity) == lastXDirection && Mathf.Sign(lastXVelocity) > 0)
        {
            lastXVelocity -= slideSpeedFalloff;
            rb.velocity = new Vector2(lastXVelocity, rb.velocity.y);
        }
        if (Mathf.Sign(lastXVelocity) == lastXDirection && Mathf.Sign(lastXVelocity) < 0)
        {
            lastXVelocity += slideSpeedFalloff;
            rb.velocity = new Vector2(lastXVelocity, rb.velocity.y);
        }

        playerCollider.size = playerColliderSlidingSize;
        playerCollider.offset = playerColliderSlidingOffset;
    }

    void EndSliding()
    {
        isSliding = false;
        playerCollider.size = playerColliderStandingSize;
        playerCollider.offset = playerColliderStandingOffset;
    }
}
