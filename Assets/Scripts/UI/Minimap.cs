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
 
        // Also set rotation equal to player rotation (in the future this should be a setting)
        Quaternion newRotation = Quaternion.Euler(90f, 0f, -Player.rotation.eulerAngles.y);
        
        this.transform.rotation = newRotation;
    }
}
