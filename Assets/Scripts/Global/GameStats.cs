using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStats 
{
    public static int NumOfPlayers { get; set; } = 1;
    public static bool[] PlayersReady { get; set; } = { false, false, false, false };

    public enum charChoices
    {
        pinky,
        songbird,
    }
    public static charChoices[] chosenChars = { charChoices.pinky, charChoices.songbird, charChoices.pinky, charChoices.songbird };

    public static int PlayersFinished { get; set; } = 0;

    public static bool GamePaused = false;

    public static int PlayerPaused = 1;
}
