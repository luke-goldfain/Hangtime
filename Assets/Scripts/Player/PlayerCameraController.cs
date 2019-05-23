using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{

    public float LookSensitivity = 3f;

    private float yaw = 0;
    private float pitch = 0;

    enum controlType
    {
        pad,
        mouse
    }
    controlType currentControlType;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentControlType = controlType.mouse;

        if (Input.GetAxis("Alt Horizontal") != 0 || Input.GetAxis("Alt Vertical") != 0)
        {
            currentControlType = controlType.pad;
        }

        switch (currentControlType)
        {
            case controlType.mouse:
                yaw += LookSensitivity * Input.GetAxis("Mouse X");
                pitch -= LookSensitivity * Input.GetAxis("Mouse Y");
                break;
            case controlType.pad:
                yaw += LookSensitivity * Input.GetAxis("Alt Horizontal");
                pitch -= LookSensitivity * Input.GetAxis("Alt Vertical");
                break;
        }

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0);

        this.GetComponentInParent<Rigidbody>().transform.eulerAngles = new Vector3(0, yaw, 0);
    }
}
