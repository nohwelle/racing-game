using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingAxe : MonoBehaviour
{
    Rigidbody2D rb;

    public float swingForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.angularVelocity = swingForce;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
