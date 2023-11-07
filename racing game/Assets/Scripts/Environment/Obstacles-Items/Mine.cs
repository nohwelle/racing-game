using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public CircleCollider2D explosionCollider;

    bool explodesOnTimer;
    float timeLeftUntilExplosion = 5f; // 5 sec by default
    float explosionDuration = 1f;

    private void Update()
    {
        if (explodesOnTimer)
        {
            timeLeftUntilExplosion -= Time.deltaTime;

            if (timeLeftUntilExplosion <= 0)
            {
                explosionDuration -= Time.deltaTime;

                // -- TO DO: update collider size while explosionDuration is above 0

            }

            if (explosionDuration <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.GetComponent<Racer>() || collision.gameObject.GetComponent<CTFRunner>()) && !explodesOnTimer)
        {
            // -- TO DO: add explosion effect

            Destroy(gameObject);
        }
    }
}
