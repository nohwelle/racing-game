using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CTFAIMovement : MonoBehaviour
{
    CTFRunner ctfRunner;

    public float intelligenceValue; // more intelligence value = more stupid
    public float maxIntelligenceValue = 0.5f;
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
    public float horizontalInputSmoothing = 0.01625f; // how much to increase/decrease horizontal input by each frame
    float horizontalInput;
    float lastHorizontalInput;
    public bool isStuckUnderSomething;
    public bool isGrounded;
    public bool isCrouching;
    public bool isSliding;

    GameObject targetToReach;

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
        ctfRunner = GetComponent<CTFRunner>();
        playerCollider = GetComponent<BoxCollider2D>();

        // set player collider sizes for standing & crouching
        playerColliderStandingSize = playerCollider.size;
        playerColliderStandingOffset = playerCollider.offset;

        playerColliderCrouchingSize = new Vector2(playerColliderStandingSize.x + crouchSize.x, playerColliderStandingSize.y + crouchSize.y);
        playerColliderCrouchingOffset = new Vector2(playerColliderStandingOffset.x + crouchOffset.x, playerColliderStandingOffset.y + crouchOffset.y);

        playerColliderSlidingSize = new Vector2(playerColliderStandingSize.x + slideSize.x, playerColliderStandingSize.y + slideSize.y);
        playerColliderSlidingOffset = new Vector2(playerColliderStandingOffset.x + slideOffset.x, playerColliderStandingOffset.y + slideOffset.y);
    }

    private void Start()
    {
        intelligenceValue = Random.Range(0, maxIntelligenceValue);
    }

    // Update is called once per frame
    void Update()
    {
        // get horizontal input with "smoothing"
        lastHorizontalInput = horizontalInput;

        horizontalInput += horizontalInputSmoothing;

        if (horizontalInput > 1 || horizontalInput < -1)
        {
            horizontalInput = Mathf.Sign(horizontalInput);
        }

        // set move speed limit
        moveSpeedLimit = moveSpeed / 5;

        // check for overhead collision
        OverheadCheck();

        // check for ground collision
        GroundCheck();

        // check if player should turn around
        FlipDirection();

        // get unstuck if sliding speed ends while too far away to do another action
        if (rb.velocity.x == 0 && isSliding)
        {
            EndSliding();
        }

        // actually apply slide speed falloff
        if (isSliding)
        {
            horizontalInput = 0;
            rb.velocity -= new Vector2(slideSpeedFalloff * Mathf.Sign(rb.velocity.x), 0);
        }
    }

    private void FixedUpdate()
    {
        // mooooove!
        if (!ctfRunner.isInHitStun && !isSliding && !isCrouching && horizontalInput != 0)
        {
            rb.velocity += new Vector2(horizontalInput * moveSpeed * Time.deltaTime, 0);
        }
        if (!ctfRunner.isInHitStun && isCrouching)
        {
            rb.velocity += new Vector2(horizontalInput * moveSpeed / 2 * Time.deltaTime, 0);
        }

        // get-out-of-slides-sooner card -- end slide if crouch-walking would be faster
        if (isSliding && Mathf.Abs(rb.velocity.x) < moveSpeedLimit / 2)
        {
            EndSliding();
        }

        // slow player down if not inputting anything
        if (!ctfRunner.isInHitStun && Mathf.Abs(horizontalInput) <= Mathf.Abs(lastHorizontalInput) && Mathf.Abs(horizontalInput) < 1 && rb.velocity.x != 0)
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
                StartCoroutine(StartCrouching());
            }
        }
        else
        {
            isStuckUnderSomething = false;
        }
    }

    void GroundCheck()
    {
        // detect if a groundLayer object is within the 0.1 unit gap between the player's collider size and groundCheck's position
        if (Physics2D.OverlapBox(groundCheck.position + (Vector3)(playerCollider.offset * transform.localScale), playerCollider.size * transform.localScale, 0, groundLayer))
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
        if (collision.gameObject.transform.parent && collision.gameObject.transform.parent.GetComponent<Obstacle>())
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
        if (collision.gameObject.transform.parent)
        {
            // if stuck behind a high obstacle and still within its trigger area, crouch-walk under it if it's shorter than the player
            if (collision.gameObject.transform.position.y > transform.position.y && collision.gameObject.transform.parent.localScale.y < transform.localScale.y && rb.velocity.x == 0)
            {
                StartCoroutine(Think(StartCrouching()));
            }

            // if stuck behind a low obstacle and still within its trigger area, jump over it
            if (collision.gameObject.transform.position.y < transform.position.y)
            {
                StartCoroutine(Think(Jump()));
            }

            // if stuck behind an obstacle and still within its trigger area, jump over it if it's taller than the player
            if (collision.gameObject.transform.parent.localScale.y > transform.localScale.y)
            {
                StartCoroutine(Think(Jump()));
            }

            if (collision.gameObject.transform.parent.GetComponent<Obstacle>())
            {
                if (isSliding && rb.velocity.x == 0)
                {
                    StopCoroutine(StartSliding());
                    EndSliding();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent && collision.gameObject.transform.parent.GetComponent<Obstacle>())
        {
            if (isSliding)
            {
                StopCoroutine(StartSliding());
                EndSliding();
            }

            if (isCrouching)
            {
                StopCoroutine(StartCrouching());
                EndCrouching();
            }
        }
    }

    IEnumerator Think(IEnumerator coroutineName)
    {
        // enforce delay on actions before executing
        float racerCount = RaceGameManager.Instance.allRacers.Count;
        yield return new WaitForSeconds(Random.Range(intelligenceValue, intelligenceValue + (racerCount - (GetComponent<Racer>().currentPlacement + 1)) / racerCount * maxIntelligenceValue));

        StartCoroutine(coroutineName);

        yield break;
    }

    IEnumerator Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

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
