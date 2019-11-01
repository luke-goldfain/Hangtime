using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script works in conjunction with a code in the PlayerContoller.
/// that code creates a wall of force that can be used to push the player in the direction determined by a trigger collider.
/// </summary>
public class ForceField : MonoBehaviour
{
    [Tooltip("Sets the strength of force effect.")]
    [SerializeField]
    public float ForceStrength;
    [Tooltip("Sets the direction this force effect moves objects in")]
    [SerializeField]
    public Vector3 ForceDirection;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Rigidbody pRb = other.gameObject.GetComponent<Rigidbody>();

            pRb.AddForce(ForceStrength * ForceDirection);
        }
    }

    //public void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        Rigidbody pRb = collision.gameObject.GetComponent<Rigidbody>();

    //        pRb.AddForce(ForceStrength * ForceDirection * 100f);
    //    }
    //}

}
