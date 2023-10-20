using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float moveSpeed;
    public float turnSpeed;

    public GameObject targetToFollow;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // switch knockback force direction based on movement
        if ((rb.velocity.x < 0 && GetComponent<Obstacle>().knockbackForce.x > 0) || (rb.velocity.x > 0 && GetComponent<Obstacle>().knockbackForce.x < 0))
        {
            // moving left or right
            GetComponent<Obstacle>().knockbackForce = new(-GetComponent<Obstacle>().knockbackForce.x, GetComponent<Obstacle>().knockbackForce.y);
        }
        if ((rb.velocity.y > 0 && GetComponent<Obstacle>().knockbackForce.y < 0) || (rb.velocity.y < 0 && GetComponent<Obstacle>().knockbackForce.y > 0))
        {
            // moving up or down
            GetComponent<Obstacle>().knockbackForce = new(GetComponent<Obstacle>().knockbackForce.x, -GetComponent<Obstacle>().knockbackForce.y);
        }
    }

    private void FixedUpdate()
    {
        if (targetToFollow)
        {
            Follow();

            // -- add rocket trail effect
        }
    }

    void Follow()
    {
        Vector3 target = targetToFollow.transform.position;
        target -= transform.position;
        target.z = 0f;
        float angle = Mathf.Atan2(target.x, target.y) * Mathf.Rad2Deg;

        rb.rotation = Mathf.Lerp(rb.rotation, -angle, turnSpeed * Time.deltaTime);
        rb.velocity = moveSpeed * Time.deltaTime * transform.up;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // -- TO DO: add explosion effect
        Destroy(gameObject);
    }
}
