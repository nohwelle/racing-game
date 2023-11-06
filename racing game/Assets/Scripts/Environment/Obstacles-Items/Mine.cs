using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Racer>())
        {
            // -- TO DO: add explosion effect

            Destroy(gameObject);
        }
    }
}
