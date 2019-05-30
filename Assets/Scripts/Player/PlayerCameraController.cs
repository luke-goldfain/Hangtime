using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    private int playerNumber;

    private int numberOfPlayers;

    private float cameraX, cameraY, cameraW, cameraH;

    private string playerLookHorizontalAxis;
    private string playerLookVerticalAxis;

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
        playerNumber = this.GetComponentInParent<PlayerController>().PlayerNumber;

        // DEBUGGY BRUTEFORCEY
        numberOfPlayers = GameObject.FindGameObjectsWithTag("Player").Length; // TODO: Change this to reference a global variable, set via menu input.

        StartAssignInputAxes();

        switch (playerNumber)
        {
            case 1:
                cameraX = 0;
                if (numberOfPlayers < 3) cameraY = 0;
                else cameraY = 0.5f;
                if (numberOfPlayers < 2) cameraW = 1f;
                else cameraW = 0.5f;
                if (numberOfPlayers < 3) cameraH = 1f;
                else cameraH = 0.5f;
                break;
            case 2:
                cameraX = 0.5f;
                if (numberOfPlayers < 3) cameraY = 0;
                else cameraY = 0.5f;
                cameraW = 0.5f;
                if (numberOfPlayers < 3) cameraH = 1f;
                else cameraH = 0.5f;
                break;
            case 3:
                cameraX = 0;
                cameraY = 0;
                cameraW = 0.5f;
                cameraH = 0.5f;
                break;
            case 4:
                cameraX = 0.5f;
                cameraY = 0;
                cameraW = 0.5f;
                cameraH = 0.5f;
                break;
        }

        this.GetComponent<Camera>().rect = new Rect(cameraX, cameraY, cameraW, cameraH);
    }

    // Update is called once per frame
    void Update()
    {
        currentControlType = controlType.mouse;

        if (Input.GetAxis(playerLookHorizontalAxis) != 0 || Input.GetAxis(playerLookVerticalAxis) != 0)
        {
            currentControlType = controlType.pad;
        }

        yaw += LookSensitivity * Input.GetAxis(playerLookHorizontalAxis);
        pitch -= LookSensitivity * Input.GetAxis(playerLookVerticalAxis);

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0);

        this.GetComponentInParent<Rigidbody>().transform.eulerAngles = new Vector3(0, yaw, 0);
    }

    private void StartAssignInputAxes()
    {
        playerLookHorizontalAxis = "P" + playerNumber + " Alt Horizontal";
        playerLookVerticalAxis = "P" + playerNumber + " Alt Vertical";
    }
}
