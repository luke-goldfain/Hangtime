using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Tooltip("List of possible powerups to be randomly selected")]
    [SerializeField]
    public List<GameObject> PowerUpList = new List<GameObject>();

    
    //public bool HoldingPickUp = false; If player has a pick-up active, this statement is true, if not it is false.

    private int choosePowerUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup();

        }
    }
    /// <summary>
    /// When Powerup is picked up a random powerup is selected and is made active on player.
    /// </summary>
    void Pickup()
    {
        Debug.Log("Power up equipped...");
        choosePowerUp = UnityEngine.Random.Range(0, PowerUpList.Count);

        Debug.Log("Choosen Power-Up: " + choosePowerUp);

       // HoldingPickUp = true; Equip power-up onto player.


        Destroy(gameObject); // Destroy power-up Object after pickup.

    }
}
