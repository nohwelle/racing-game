using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public KeyCode jumpKey;
    public KeyCode crouchKey;

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
        if (isGrounded && Input.GetKeyDown(jumpKey) && (!isSliding || !isCrouching))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        // start or end crouch/slide
        if (isGrounded && Input.GetKey(crouchKey) && rb.velocity.x == 0)
        {
            StartCrouching();
        }
        if (isCrouching && Input.GetKeyUp(crouchKey))
        {
            EndCrouching();
        }
        if (isGrounded && Input.GetKey(crouchKey) && rb.velocity.x != 0 && !isCrouching)
        {
            StartSliding();
        }
        if (isSliding && (Input.GetKeyUp(crouchKey) || rb.velocity.x == 0))
        {
            EndSliding();
        }

        // check if player should turn around
        FlipDirection();
    }

    private void FixedUpdate()
    {
        // slow player down if not inputting anything
        if (horizontalInput == 0)
        {
            if (!isSliding)
            {
                rb.velocity -= new Vector2(moveFriction * Mathf.Sign(rb.velocity.x), 0);
            }

            if (rb.velocity.x <= moveFriction && rb.velocity.x >= -moveFriction)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        if (!isSliding && !isCrouching && horizontalInput != 0)
        {
            float lastHorizontalInput = horizontalInput;
            if (horizontalInput != lastHorizontalInput && Mathf.Abs(horizontalInput) > 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            rb.velocity += new Vector2(horizontalInput * moveSpeed * Time.deltaTime, 0);
        }
        if (isCrouching)
        {
            rb.velocity += new Vector2(horizontalInput * moveSpeed / 2 * Time.deltaTime, 0);
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
        horizontalInput = 0;

        rb.velocity -= new Vector2(slideSpeedFalloff * Mathf.Sign(rb.velocity.x), 0);

        playerCollider.size = playerColliderSlidingSize;
        playerCollider.offset = playerColliderSlidingOffset;
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
