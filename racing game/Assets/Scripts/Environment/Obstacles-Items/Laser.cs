using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer laserBeam;
    public Transform laserEndPoint;
    public Transform gunBarrel;
    public BoxCollider2D laserCollider;
    public BoxCollider2D laserTrigger;

    public Vector2 firingDirection;
    public float laserBeamMaxLength;

    public float firingDuration;
    public float firingCooldown;

    public float laserTriggerSize;

    public bool isFiring;
    bool hasFired = true;
    bool hasSetTrigger;

    // Update is called once per frame
    void Update()
    {
        // rotato ... bananba
        if (transform.eulerAngles.z == 0) // firing upwards
        {
            firingDirection = new(0, 1);
        }
        if (transform.eulerAngles.z == 90) // firing to the left
        {
            firingDirection = new(-1, 0);
        }
        if (transform.eulerAngles.z == 180) // firing downwards
        {
            firingDirection = new(0, -1);
        }
        if (transform.eulerAngles.z == 270) // firing to the right
        {
            firingDirection = new(1, 0);
        }

        // handle laser firing
        if (isFiring && !hasFired)
        {
            StartCoroutine(StartFiring());
        }

        if (!isFiring && hasFired)
        {
            StartCoroutine(StopFiring());
        }

        if (!hasSetTrigger)
        {
            StartCoroutine(SetTrigger());
        }
    }

    IEnumerator StartFiring()
    {
        // shoot da ting
        hasFired = true;

        // -- TO DO: figure out what laser should do if beam collides with a player at any point
        RaycastHit2D hit = Physics2D.Raycast(transform.position, firingDirection, Mathf.Infinity, ~3);

        if (hit.collider)
        {
            laserEndPoint.position = hit.point;
        }
        
        if (!hit.collider || new Vector2(Mathf.Abs(hit.point.x - transform.position.x), Mathf.Abs(hit.point.y - transform.position.y)).magnitude > laserBeamMaxLength)
        {
            if (transform.eulerAngles.z == 0)
            {
                laserEndPoint.position = new(laserEndPoint.position.x, transform.position.y + laserBeamMaxLength);
            }
            if (transform.eulerAngles.z == 90)
            {
                laserEndPoint.position = new(transform.position.x - laserBeamMaxLength, laserEndPoint.position.y);
            }
            if (transform.eulerAngles.z == 180)
            {
                laserEndPoint.position = new(laserEndPoint.position.x, transform.position.y - laserBeamMaxLength);
            }
            if (transform.eulerAngles.z == 270)
            {
                laserEndPoint.position = new(transform.position.x + laserBeamMaxLength, laserEndPoint.position.y);
            }
        }

        laserBeam.SetPosition(0, transform.position);
        laserBeam.SetPosition(1, laserEndPoint.position);

        laserCollider.size = new(laserBeam.endWidth, laserEndPoint.localPosition.y);
        laserCollider.offset = new(0, laserEndPoint.localPosition.y / 2);

        yield return new WaitForSeconds(firingDuration);

        isFiring = false;

        yield break;
    }

    IEnumerator StopFiring()
    {
        // stop shootin da ting
        hasFired = false;
        hasSetTrigger = false;

        laserEndPoint.position = transform.position;

        laserBeam.SetPosition(0, transform.position);
        laserBeam.SetPosition(1, laserEndPoint.position);

        laserCollider.size = new(0.1f, 0.1f);
        laserCollider.offset = new(0, 0);

        // -- TO DO: create laser charge-up effect to show players when laser will fire

        yield return new WaitForSeconds(firingCooldown);

        isFiring = true;

        yield break;
    }

    IEnumerator SetTrigger()
    {
        // set trigger to warn AI of ting before it shoots
        laserTrigger.size = new(0.1f, 0.1f);
        laserTrigger.offset = new(0, 0);

        yield return new WaitForSeconds(firingDuration / 2);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, firingDirection, Mathf.Infinity, ~3);

        laserEndPoint.position = hit.point;

        laserTrigger.size = new(laserTriggerSize, laserEndPoint.localPosition.y + 1);
        laserTrigger.offset = new(0, laserEndPoint.localPosition.y / 2);

        laserEndPoint.position = transform.position;

        hasSetTrigger = true;

        yield break;
    }
}
