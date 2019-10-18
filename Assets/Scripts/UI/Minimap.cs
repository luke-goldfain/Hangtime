using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script allows the minimap to center the player in the scene.
/// </summary>
public class Minimap : MonoBehaviour
{
    public Transform Player;

    private void LateUpdate()
    {
        Vector3 newPosition = Player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;


    }
}
