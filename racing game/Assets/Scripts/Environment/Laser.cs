using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer laserBeam;
    public Transform laserEndPoint;
    public Transform gunBarrel;
    public BoxCollider2D laserCollider;

    public Vector2 firingDirection;

    public float firingDuration;
    public float firingCooldown;

    public bool isFiring;
    bool hasFired;
    bool hasHit;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LaserOperation());
    }

    // Update is called once per frame
    void Update()
    {
        if (isFiring && !hasFired)
        {
            StartCoroutine(LaserOperation());
        }

        if (!isFiring && hasFired)
        {
            StartCoroutine(LaserOperation());
        }
    }

    IEnumerator LaserOperation()
    {
        if (isFiring)
        {
            hasFired = true;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, firingDirection, Mathf.Infinity, ~3);

            if (!hasHit && hit.collider)
            {
                laserEndPoint.position = hit.point;
                hasHit = true;
            }

            laserBeam.SetPosition(0, transform.position);
            laserBeam.SetPosition(1, laserEndPoint.position);

            laserCollider.size = new(laserBeam.endWidth, transform.position.x - laserEndPoint.position.x);
            laserCollider.offset = new(0, transform.position.x - laserEndPoint.position.x);

            yield return new WaitForSeconds(firingDuration);

            isFiring = false;

            yield break;
        }

        if (!isFiring)
        {
            hasFired = false;

            laserEndPoint.position = transform.position;

            laserBeam.SetPosition(0, transform.position);
            laserBeam.SetPosition(1, laserEndPoint.position);

            laserCollider.size = new(0.1f, 0.1f);
            laserCollider.offset = new(0, 0);

            yield return new WaitForSeconds(firingCooldown);

            hasHit = false;
            isFiring = true;

            yield break;
        }
    }
}
