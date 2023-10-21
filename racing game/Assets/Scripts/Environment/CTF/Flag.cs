using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.Rendering;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public bool isBeingCaptured;
    public float distanceFromCapturer = 0.5f;

    [SerializeField] private float followSpeed = 50;
    GameObject currentCapturer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CTFRunner>())
        {
            isBeingCaptured = true;
            currentCapturer = collision.gameObject;
        }
    }

    private void FixedUpdate()
    {
        if (isBeingCaptured)
        {
            Vector3 distance = currentCapturer.transform.position - transform.position;

            if (distance.magnitude >= distanceFromCapturer)
            {
                Vector3 targetPoint = currentCapturer.transform.position - distance.normalized * distanceFromCapturer;

                transform.position = Vector3.Lerp(transform.position, Vector3.MoveTowards(transform.position, targetPoint, followSpeed * Time.deltaTime), followSpeed * Time.deltaTime);
            }
        }
    }
}
