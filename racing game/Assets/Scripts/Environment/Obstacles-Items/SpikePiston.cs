using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikePiston : MonoBehaviour
{
    Rigidbody2D rb;
    public Vector2 originPosition;
    public LineRenderer pistonPole;

    public Vector2 extensionSpeed;
    public Vector2 retractionSpeed;
    public float retractionDelay = 0.75f;

    public float extensionDistanceLimit = 5f;

    public bool isExtending;
    public bool isRetracting;
    public bool hasHitExtensionLimit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        originPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // make pole follow piston head
        pistonPole.SetPosition(0, originPosition);
        pistonPole.SetPosition(1, transform.position);

        // determine current rotation, then use it to modify extension speed (and thus direction) + extension distance target
        if (transform.eulerAngles.z == 0) // rotated facing upwards
        {
            extensionSpeed = new(0, 0.05f);
            retractionSpeed = new(0, -2);
        }
        if (transform.eulerAngles.z == 90) // rotated facing to the left
        {
            extensionSpeed = new(0.05f, 0);
            retractionSpeed = new(-2, 0);
        }
        if (transform.eulerAngles.z == 180) // rotated facing downwards
        {
            extensionSpeed = new(0, -0.05f);
            retractionSpeed = new(0, 2);
        }
        if (transform.eulerAngles.z == 270) // rotated facing to the right
        {
            extensionSpeed = new(-0.05f, 0);
            retractionSpeed = new(2, 0);
        }


        if (isExtending)
        {
            rb.velocity += extensionSpeed;
            StopPistonAtExtensionLimit();
        }

        if (isRetracting)
        {
            rb.velocity = retractionSpeed;
            StopPistonAtRetractionLimit();
        }

        if (!isExtending && !isRetracting)
        {
            rb.velocity = Vector2.zero;
            hasHitExtensionLimit = false;
        }

        // DEBUG: if piston is somehow extending *and* retracting, tell it to knock that off
        if (isExtending && isRetracting)
        {
            isExtending = false;
            isRetracting = false;
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.GetComponent<Racer>() || collision.gameObject.GetComponent<CTFRunner>()) && !isExtending && !isRetracting)
        {
            // make piston extend
            isExtending = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // stop piston if it collides with ground or player before reaching its extension limit
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            if (!hasHitExtensionLimit)
            {
                rb.velocity = Vector2.zero;
                StartCoroutine(DelayRetract());
                isExtending = false;
            }
        }
    }

    void StopPistonAtExtensionLimit()
    {
        // stop piston once it has reached its extension limit
        if (transform.eulerAngles.z == 0 && transform.position.y >= originPosition.y + extensionDistanceLimit)
        {
            rb.velocity = Vector2.zero;
            StartCoroutine(DelayRetract());
            hasHitExtensionLimit = true;
            isExtending = false;
        }
        if (transform.eulerAngles.z == 90 && transform.position.x >= originPosition.x + extensionDistanceLimit)
        {
            rb.velocity = Vector2.zero;
            StartCoroutine(DelayRetract());
            hasHitExtensionLimit = true;
            isExtending = false;
        }
        if (transform.eulerAngles.z == 180 && transform.position.y <= originPosition.y - extensionDistanceLimit)
        {
            rb.velocity = Vector2.zero;
            StartCoroutine(DelayRetract());
            hasHitExtensionLimit = true;
            isExtending = false;
        }
        if (transform.eulerAngles.z == 270 && transform.position.x <= originPosition.x - extensionDistanceLimit)
        {
            rb.velocity = Vector2.zero;
            StartCoroutine(DelayRetract());
            hasHitExtensionLimit = true;
            isExtending = false;
        }
    }

    void StopPistonAtRetractionLimit()
    {
        // stop piston once it has reached its origin position
        if (transform.eulerAngles.z == 0 && transform.position.y <= originPosition.y)
        {
            rb.velocity = Vector2.zero;
            isRetracting = false;
        }
        if (transform.eulerAngles.z == 90 && transform.position.x <= originPosition.x)
        {
            rb.velocity = Vector2.zero;
            isRetracting = false;
        }
        if (transform.eulerAngles.z == 180 && transform.position.y >= originPosition.y)
        {
            rb.velocity = Vector2.zero;
            isRetracting = false;
        }
        if (transform.eulerAngles.z == 270 && transform.position.x >= originPosition.x)
        {
            rb.velocity = Vector2.zero;
            isRetracting = false;
        }
    }

    IEnumerator DelayRetract()
    {
        yield return new WaitForSeconds(retractionDelay);

        isRetracting = true;

        yield break;
    }
}
