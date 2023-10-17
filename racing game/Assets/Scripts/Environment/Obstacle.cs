using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector2 knockbackForce;
    public float hitStunTime;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // apply knockback & hitstun
        if (collision.gameObject.GetComponent<Racer>())
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(collision.gameObject.GetComponent<Racer>().HitStunCooldown(hitStunTime));
        }
    }
}
