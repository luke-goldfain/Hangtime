using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectPanel : MonoBehaviour
{
    [SerializeField]
    private int playerNumber;

    [SerializeField]
    private GameObject playerModel;

    [SerializeField]
    private GameObject playerConfirmedText;

    [SerializeField]
    private GameObject controlsPanel;

    private bool panelActive;
    private bool acceptsInput;

    // Update is called once per frame
    void Update()
    {
        acceptsInput = !controlsPanel.activeInHierarchy;

        if (Input.GetButtonDown("P" + playerNumber + "Jump") && !panelActive && acceptsInput)
        {
            ActivatePlayerPanel();
        }

        if (Input.GetButtonDown("P" + playerNumber + "Cancel") && panelActive && acceptsInput)
        {
            DeactivatePlayerPanel();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Minus) && playerNumber == 1)
        {
            ActivatePlayerPanel();
        }

        if (Input.GetKeyDown(KeyCode.Equals) && playerNumber == 2)
        {
            ActivatePlayerPanel();
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket) && playerNumber == 3)
        {
            ActivatePlayerPanel();
        }

        if (Input.GetKeyDown(KeyCode.RightBracket) && playerNumber == 4)
        {
            ActivatePlayerPanel();
        }
#endif

        if (playerModel.activeInHierarchy)
        {
            playerModel.transform.Rotate(0f, 2f, 0f);
        }
    }

    private void ActivatePlayerPanel()
    {
        panelActive = true;

        playerConfirmedText.SetActive(true);

        playerModel.SetActive(true);

        GameStats.PlayersReady[playerNumber - 1] = true;
    }

    private void DeactivatePlayerPanel()
    {
        panelActive = false;

        playerConfirmedText.SetActive(false);

        playerModel.SetActive(false);

        GameStats.PlayersReady[playerNumber - 1] = false;
    }
}
