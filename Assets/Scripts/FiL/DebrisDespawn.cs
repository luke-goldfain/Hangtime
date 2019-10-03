using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisDespawn : MonoBehaviour
{

    private bool boost = false;
    public Rigidbody rb;
    private float jumpstart = -50;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boost == false)
        {
            rb.AddForce(transform.up * jumpstart);
            boost = true;
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
