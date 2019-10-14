using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatWalk : MonoBehaviour
{

    [Tooltip("Amount that stats will be multiplied by.")]
    [SerializeField]
    private float multiplier = 1.6f;

    [Tooltip("The length of time this power-up will be in effect on player.")]
    [SerializeField]
    private float Duration = 4f;

    [SerializeField]
    private GameObject pickupVFX;
    //public bool HoldingPowerUp = false; If player has a pick-up active, this statement is true, if not it is false.

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine( Pickup(other) );

        }
    }
    /// <summary>
    /// When Powerup is picked up a random powerup is selected and is made active on player.
    /// </summary>
    IEnumerator Pickup(Collider Player)
    {
        Debug.Log("Power up equipped...");

        Instantiate(pickupVFX, transform.position, transform.rotation);

        PlayerController stats = Player.GetComponent<PlayerController>();
            stats.DefaultRunSpeed *= multiplier;
            stats.JumpForce *= multiplier;

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(Duration); //wait for a set amount of time before ending effect.
        stats.DefaultRunSpeed /= multiplier;
        stats.JumpForce /= multiplier;


        Destroy(gameObject); // Destroy power-up Object after pickup.

    }
}
