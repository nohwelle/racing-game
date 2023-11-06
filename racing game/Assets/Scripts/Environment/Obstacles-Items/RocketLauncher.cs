using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RocketLauncher : MonoBehaviour
{
    public GameObject rocket;
    public GameObject gunBarrel;
    public Transform firingPoint;
    public BoxCollider2D rocketFireTrigger;

    public float gunBarrelAimSpeed;

    public float cooldownTime;
    public bool isOnCooldown;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Racer>())
        {
            Vector3 target = collision.gameObject.transform.position;
            target -= gunBarrel.transform.position;
            target.z = 0f;

            float angle = Mathf.Atan2(target.x, target.y) * Mathf.Rad2Deg;
            gunBarrel.transform.rotation = Quaternion.Lerp(gunBarrel.transform.rotation, Quaternion.Euler(new Vector3(0, 0, -angle)), gunBarrelAimSpeed * Time.deltaTime);


            if (!isOnCooldown)
            {
                FireRocket(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Racer>())
        {
            gunBarrel.transform.rotation = Quaternion.Lerp(gunBarrel.transform.rotation, Quaternion.identity, gunBarrelAimSpeed * Time.deltaTime);
        }
    }

    void FireRocket(GameObject target)
    {
        Instantiate(rocket, firingPoint.position, gunBarrel.transform.rotation);
        rocket.GetComponent<Rocket>().targetToFollow = target;

        isOnCooldown = true;
        StartCoroutine(StartCooldown());
    }

    IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);

        isOnCooldown = false;

        yield break;
    }
}
