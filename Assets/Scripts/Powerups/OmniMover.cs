using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Omni mover will allow the player who aquires this powerup to ignore there mass and send themselves flying in the direction they are looking.
/// This should be a quick application of force that will be used to offer players more air control by switching the direction of movement at will.
/// Currently commented out: Impact force for the effect of colliding with other players using this effect. 
/// Currently commented out: Stun duration is a result of colliding head on with another player on the opponent.
/// Currently commented out: Bounce back is the result of colliding head on with another player on the user.
/// Currently commented out: Inner hitbox triggers knockback and bounce back.
/// Currently commented out: Outer hit box triggers stun on hit.
/// </summary>
public class OmniMover : MonoBehaviour
{
    [Tooltip("The forward momentum when the ability is triggered.")]
    [SerializeField]
    private float MoveForce = 12.8f; //Move player with force equal to number in editior.
    [Tooltip("The knockback force of the movement.")]
    //[SerializeField]
    //private float ImpactForce = 2.8f;
    //[Tooltip("The duration of the stun effect.")]
    //[SerializeField]
    //private float StunDuration = 2f;
    //[Tooltip("The distance that the player bounces back when they collide with another game object.")]
    //[SerializeField]
    //private float BounceBack = 2.1f;

    //[Tooltip("Second hitbox that has a knockback effect on other players and triggers the bounce effect on user.")]
    //[SerializeField]
    //private GameObject InnerHitbox;
    //[Tooltip("First hitbox has a stunning effect that pushes players aside.")]
    //[SerializeField]
    //private GameObject OuterHitbox;

    [SerializeField]
    public GameObject Player;

    [Tooltip("Visual effect that plays on pick up.")]
    [SerializeField]
    private GameObject pickupVFX;

    private Rigidbody rb;
    /// <summary>
    /// The OnTriggerEnter method is called when the player passes though the power-up. It should allow the player to pickup the
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
      
        if (other.CompareTag("Player"))
        {
            Pickup(other);
            rb = Player.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.forward * MoveForce, ForceMode.Acceleration); // Moves player forward when they use th powerup.
        }
    }
    /// <summary>
    /// This equips the powerup for later use.
    /// </summary>
    void Pickup(Collider Player)
    {
        Debug.Log("Power up equipped...");

        Instantiate(pickupVFX, transform.position, transform.rotation);

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false; 

        Destroy(gameObject); // Destroy power-up Object after pickup.
    }
}
