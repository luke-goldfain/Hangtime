using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilityPowerUp : MonoBehaviour
{
    public float multiplier = 1.6f;
    
    void ActivePowerUp(Collider player)
    {
        PlayerController stats = player.GetComponent<PlayerController>();
        stats.DefaultRunSpeed *= multiplier;
        stats.JumpForce *= multiplier;
    }


}
