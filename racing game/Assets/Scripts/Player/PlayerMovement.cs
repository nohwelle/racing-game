using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    Racer racer;

    public KeyCode jumpKey;
    public KeyCode crouchKey;

    public float moveSpeed = 16.25f;
    float moveSpeedLimit;
    public float moveFriction = 0.1625f;
    public float jumpSpeed = 4.25f;
    public float slideSpeedFalloff = 0.0075f;
    public Vector2 crouchSize;
    public Vector2 crouchOffset;
    public Vector2 slideSize;
    public Vector2 slideOffset;

    bool isFacingLeft;
    float horizontalInput;
    float lastHorizontalInput;
    public bool isStuckUnderSomething;
    public bool isGrounded;
    public bool isCrouching;
    public bool isSliding;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform overheadCheck;
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
        racer = GetComponent<Racer>();
        playerCollider = GetComponent<BoxCollider2D>();

        // set player collider sizes for standing & crouching
        playerColliderStandingSize = playerCollider.size;
        playerColliderStandingOffset = playerCollider.offset;

        playerColliderCrouchingSize = new Vector2(playerColliderStandingSize.x + crouchSize.x, playerColliderStandingSize.y + crouchSize.y);
        playerColliderCrouchingOffset = new Vector2(playerColliderStandingOffset.x + crouchOffset.x, playerColliderStandingOffset.y + crouchOffset.y);

        playerColliderSlidingSize = new Vector2(playerColliderStandingSize.x + slideSize.x, playerColliderStandingSize.y + slideSize.y);
        playerColliderSlidingOffset = new Vector2(playerColliderStandingOffset.x + slideOffset.x, playerColliderStandingOffset.y + slideOffset.y);

        // set value of horizontal input so lastHorizontalInput can be set before next update
        horizontalInput = Input.GetAxis("Horizontal");
    }

    // Update is called once per frame
    void Update()
    {
        // get horizontal input with smoothing
        lastHorizontalInput = horizontalInput;
        horizontalInput = Input.GetAxis("Horizontal");

        // set move speed limit
        moveSpeedLimit = moveSpeed / 5;

        // check for overhead collision
        OverheadCheck();

        // check for ground collision
        GroundCheck();

        // perform jump
        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        // start or end crouch/slide
        if (isGrounded && Input.GetKey(crouchKey) && rb.velocity.x == 0)
        {
            StartCrouching();
        }
        if ((isCrouching && !isStuckUnderSomething && Input.GetKeyUp(crouchKey)) || (!isStuckUnderSomething && isCrouching && !Input.GetKey(crouchKey)))
        {
            EndCrouching();
        }
        if (isGrounded && Input.GetKey(crouchKey) && rb.velocity.x != 0 && !isCrouching)
        {
            StartSliding();
        }
        if ((isSliding && !isStuckUnderSomething && Input.GetKeyUp(crouchKey)) || (isSliding && rb.velocity.x == 0))
        {
            EndSliding();
        }

        // check if player should turn around
        FlipDirection();
    }

    private void FixedUpdate()
    {
        // mooooove!
        if (!racer.isInHitStun && !isSliding && !isCrouching && horizontalInput != 0)
        {
            rb.velocity += new Vector2(horizontalInput * moveSpeed * Time.deltaTime, 0);
        }
        if (!racer.isInHitStun && isCrouching)
        {
            rb.velocity += new Vector2(horizontalInput * moveSpeed / 2 * Time.deltaTime, 0);
        }

        // slow player down if not inputting anything
        if (!racer.isInHitStun && Mathf.Abs(horizontalInput) <= Mathf.Abs(lastHorizontalInput) && Mathf.Abs(horizontalInput) < 1 && rb.velocity.x != 0)
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

    void OverheadCheck()
    {
        // detect if a groundLayer object is within the gap between the player's collider size and overheadCheck's position
        if (Physics2D.OverlapBox(overheadCheck.position + (Vector3)(playerCollider.offset * transform.localScale), playerCollider.size * overheadCheck.localScale * transform.localScale, 0, groundLayer))
        {
            isStuckUnderSomething = true;
            
            if (isGrounded)
            {
                StartCrouching();
            }
        }
        else
        {
            isStuckUnderSomething = false;
        }
    }

    void GroundCheck()
    {
        // detect if a groundLayer object is within the gap between the player's collider size and groundCheck's position
        if (Physics2D.OverlapBox(groundCheck.position + (Vector3)(playerCollider.offset * transform.localScale), playerCollider.size * transform.localScale, 0, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (playerCollider && groundCheck)
        {
            Gizmos.DrawWireCube(groundCheck.position + (Vector3)(playerCollider.offset * groundCheck.localScale * transform.localScale), playerCollider.size * groundCheck.localScale * transform.localScale);
        }

        if (playerCollider && overheadCheck)
        {
            Gizmos.DrawWireCube(overheadCheck.position + (Vector3)(playerCollider.offset * transform.localScale), playerCollider.size * overheadCheck.localScale * transform.localScale);
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
