using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [HideInInspector]
    public bool IsFinished;

    public float TimerPerNumber;

    private float currentTimer;

    private int currentTextIndex;

    private List<string> countdownText;

    // Start is called before the first frame update
    void Start()
    {
        countdownText = new List<string>()
        {
            "3...", "2...", "1...", "SWING!",
        };

        currentTextIndex = 0;

        IsFinished = false;

        currentTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure this doesn't overwrite FinishedResetDisplay
        if (GameStats.PlayersFinished < GameStats.NumOfPlayers)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = countdownText[currentTextIndex];
        }

        currentTimer += Time.deltaTime;


        if (currentTimer >= TimerPerNumber && currentTextIndex < (countdownText.Count - 1))
        {
            currentTextIndex++;

            currentTimer = 0f;

            AkSoundEngine.PostEvent("Countdown", GameObject.Find("Main Camera"));
        }
        else if (currentTimer >= TimerPerNumber && GameStats.PlayersFinished < GameStats.NumOfPlayers)
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }

        if (currentTextIndex == (countdownText.Count - 1))
        {
            IsFinished = true;
        }
    }
}
