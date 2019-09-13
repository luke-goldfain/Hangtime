using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStats 
{
    public static int NumOfPlayers { get; set; } = 1;
    public static bool[] PlayersReady { get; set; } = { false, false, false, false };
}
