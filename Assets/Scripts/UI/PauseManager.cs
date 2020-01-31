using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    public GameObject PauseMenu;

    // refers to the PlayerNumber variable of PlayerController
    private int pNum;

    // refers to the DesertSequence variable CutsceneIsPlaying
    private bool cutscenePlaying;
    // refers to DesertSequence itself
    [SerializeField]
    private DesertSequence dSeq;

    // Start is called before the first frame update
    void Start()
    {
        pNum = this.gameObject.GetComponent<PlayerController>().PlayerNumber;

        dSeq = FindObjectOfType(typeof(DesertSequence)) as DesertSequence;
    }

    // Update is called once per frame
    void Update()
    {
        cutscenePlaying = dSeq.CutsceneIsPlaying;

        if (Input.GetButtonDown("P" + pNum + "Start") && GameStats.PlayersFinished < GameStats.NumOfPlayers && !GameStats.GamePaused && !cutscenePlaying)
        {
            //PauseGame(); // TODO: Not currently functional
                           // Time.timeScale sets to 0, but the PauseMenu does not set active and objects not set to Time-based motion still move.
        }
    }

    private void PauseGame()
    {
        GameStats.GamePaused = true;

        GameStats.PlayerPaused = pNum;

        PauseMenu.SetActive(true);
    }
}
