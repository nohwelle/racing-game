using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public KeyCode jumpKey;
    public KeyCode crouchKey;

    public float moveSpeed;
    public float moveSpeedLimit;
    public float jumpSpeed;
    public float crouchingHeightPercent = 0.5f;
    public float crouchingWidthPercent = 1;

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


    private void Awake()
    {
        playerCollider = GetComponent<BoxCollider2D>();

        // set player collider sizes for standing & crouching
        playerColliderStandingSize = playerCollider.size;
        playerColliderStandingOffset = playerCollider.offset;

        playerColliderCrouchingSize = new Vector2(playerColliderStandingSize.x * crouchingWidthPercent, playerColliderStandingSize.y * crouchingHeightPercent);
        playerColliderCrouchingOffset = new Vector2(playerColliderStandingOffset.x * crouchingWidthPercent, playerColliderStandingOffset.y - crouchingHeightPercent / 2);
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
            lastXVelocity -= 0.01f;
            rb.velocity = new Vector2(lastXVelocity, rb.velocity.y);
        }
        if (Mathf.Sign(lastXVelocity) == lastXDirection && Mathf.Sign(lastXVelocity) < 0)
        {
            lastXVelocity += 0.01f;
            rb.velocity = new Vector2(lastXVelocity, rb.velocity.y);
        }

        playerCollider.size = playerColliderCrouchingSize;
        playerCollider.offset = playerColliderCrouchingOffset;
    }

    void EndSliding()
    {
        isSliding = false;
        playerCollider.size = playerColliderStandingSize;
        playerCollider.offset = playerColliderStandingOffset;
    }
}
