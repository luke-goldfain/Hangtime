using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPositionReset : MonoBehaviour
{
    [SerializeField]
    private Vector3 resetLocation;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Reset position every frame unless sliding
        if (!anim.GetBool("ButtonDown"))
        {
            anim.gameObject.transform.localPosition = resetLocation;
        }

        anim.gameObject.transform.localRotation = Quaternion.identity;
    }
}
