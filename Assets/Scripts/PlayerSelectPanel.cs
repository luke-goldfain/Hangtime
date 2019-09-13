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

    private bool panelActive;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("P" + playerNumber + "Jump") && !panelActive)
        {
            ActivatePlayerPanel();
        }

        if (Input.GetButtonDown("P" + playerNumber + "Cancel") && panelActive)
        {
            DeactivatePlayerPanel();
        }

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
