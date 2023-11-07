using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector2 knockbackForce = new(-5, 3);
    public float hitStunDuration = 0.25f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // apply knockback & hitstun
        if (collision.gameObject.GetComponent<Racer>() && hitStunDuration > 0)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockbackForce, ForceMode2D.Impulse);
            collision.gameObject.GetComponent<Racer>().hitStunDuration = hitStunDuration;
            collision.gameObject.GetComponent<Racer>().isInHitStun = true;
        }


        // -- TO DO: make obstacles knock back CTFRunners based on direction they're facing; if facing right, hit left & vice versa
        if (collision.gameObject.GetComponent<CTFRunner>() && hitStunDuration > 0)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockbackForce, ForceMode2D.Impulse);
            collision.gameObject.GetComponent<CTFRunner>().hitStunDuration = hitStunDuration;
            collision.gameObject.GetComponent<CTFRunner>().isInHitStun = true;
        }
    }
}
