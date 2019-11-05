using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishedResetDisplay : MonoBehaviour
{
    [Tooltip("The text that displays once all players have crossed the finish, prompting players to restart.")]
    public string ResetText;

    private bool resetTextDisplayed;

    private SetupManager setupMgr;

    // Start is called before the first frame update
    void Start()
    {
        setupMgr = this.gameObject.GetComponentInParent<SetupManager>();

        resetTextDisplayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!resetTextDisplayed && GameStats.PlayersFinished >= GameStats.NumOfPlayers)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = ResetText;

            resetTextDisplayed = true;
        }

        if (resetTextDisplayed && (Input.GetButtonDown("P1Start") ||
                                   Input.GetButtonDown("P2Start") ||
                                   Input.GetButtonDown("P3Start") ||
                                   Input.GetButtonDown("P4Start")))
        {
            setupMgr.ResetGameStatsPlayers();

            setupMgr.LoadScene("StartScreen");
        }
    }
}
