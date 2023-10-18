using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikePiston : MonoBehaviour
{
    public LineRenderer pistonPole;

    public Vector2 extensionSpeed;
    public Vector2 retractionSpeed;
    public float retractionDelay = 0.75f;

    Rigidbody2D rb;
    Vector2 originPosition;

    public bool isExtending;
    public bool isRetracting;

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

        if (isExtending)
        {
            rb.velocity += extensionSpeed;
        }

        if (isRetracting)
        {
            rb.velocity = retractionSpeed;
        }

        if (!isExtending && !isRetracting)
        {
            rb.velocity = Vector2.zero;
        }

        if (isExtending && isRetracting)
        {
            isExtending = false;
            isRetracting = false;
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Racer>() && !isExtending)
        {
            // make piston extend
            if (!isRetracting)
            {
                isExtending = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (isExtending)
            {
                StartCoroutine(DelayRetract());
                isExtending = false;
            }

            if (isRetracting)
            {
                isRetracting = false;
            }
        }
    }

    IEnumerator DelayRetract()
    {
        yield return new WaitForSeconds(retractionDelay);

        isRetracting = true;

        yield break;
    }
}
