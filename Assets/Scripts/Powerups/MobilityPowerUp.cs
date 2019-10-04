using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilityPowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    void ActivePowerUp(Collider player)
    {
        PlayerController stats = player.GetComponent<PlayerController>();
        stats.DefaultRunSpeed = 30;
        stats.JumpForce = 20;
    }


}
