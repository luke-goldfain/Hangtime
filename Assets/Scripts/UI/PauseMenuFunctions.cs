using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuFunctions : MonoBehaviour
{
    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private SetupManager setupMgr;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("P" + GameStats.PlayerPaused + "Start") ||
            Input.GetButtonDown("P" + GameStats.PlayerPaused + "Cancel"))
        {
            UnpauseGame();
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        resumeButton.Select();
    }

    public void UnpauseGame()
    {
        GameStats.GamePaused = false;

        Time.timeScale = 1f;

        this.gameObject.SetActive(false);
    }

    public void ResetGame()
    {
        GameStats.GamePaused = false;

        Time.timeScale = 1f;

        setupMgr.ResetGameStatsPlayers();

        setupMgr.LoadScene("StartScreen");
    }
}
