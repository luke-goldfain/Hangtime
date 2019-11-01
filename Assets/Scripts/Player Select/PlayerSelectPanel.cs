using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private Transform playerLocation;

    [SerializeField]
    private GameObject SelectableComponent;

    private bool panelActive;
    private bool acceptsInput;
    private bool modelInserted;

    private void Start()
    {
        modelInserted = false;

        //this.SelectableComponent.GetComponent<Selectable>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        acceptsInput = !controlsPanel.activeInHierarchy;

        if (Input.GetButtonDown("P" + playerNumber + "Jump") && !panelActive && acceptsInput)
        {
            ActivatePlayerPanel();

            AkSoundEngine.PostEvent("Select", gameObject);
        }

        if (Input.GetButtonDown("P" + playerNumber + "Cancel") && panelActive && acceptsInput)
        {
            DeactivatePlayerPanel();

            AkSoundEngine.PostEvent("Deselect", gameObject);
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
            playerModel.transform.Rotate(0f, 0.25f, 0f);
        }
    }

    private void ActivatePlayerPanel()
    {
        panelActive = true;

        playerConfirmedText.SetActive(true);

        this.SelectableComponent.GetComponent<Selectable>().interactable = true;

        this.SelectableComponent.GetComponent<Selectable>().Select();

        if (!modelInserted)
        {
            playerModel = Instantiate(playerModel, playerLocation);

            playerModel.transform.localScale = new Vector3(50f, 50f, 50f);

            modelInserted = true;
        }

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
