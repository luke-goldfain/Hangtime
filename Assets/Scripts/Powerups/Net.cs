using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fires projectile that prevents other players from moving for some time if hit.
/// Must be able to access target players playercontroller and disable movement for a certain amount of time.
/// </summary>
public class Net : MonoBehaviour
{
    [Tooltip("The projectile that is fired when the net powerup is active.")]
    [SerializeField]
   private GameObject NetProjectile;
    [Tooltip("VFX on net model.")]
    [SerializeField]
    private GameObject NetVisual;

    [SerializeField]
    public GameObject Player;

    [Tooltip("Visual effect that plays on pick up.")]
    [SerializeField]
    private GameObject pickupVFX;

    [Tooltip("The length of time the target will be trapped.")]
    [SerializeField]
    private float Duration;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            StartCoroutine(Pickup(other));
        }
    }
    IEnumerator Pickup(Collider Player)
    {
        Debug.Log("Power up equipped...");

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(Duration); //wait for a set amount of time before ending effect.

        Destroy(gameObject); // Destroy power-up Object after pickup.
    }
}
