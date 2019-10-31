using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the animations of character
/// </summary>
public class AnimCharacterController : MonoBehaviour
{
    public float speed;
    public float gravity;
    Vector3 moveDir = Vector3.zero;

      CharacterController controller;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController> ();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.W))
            {
                moveDir = new Vector3(0, 0, 1);

            }
        }
    }
}
