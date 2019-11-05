using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour
{
    [Tooltip("(Optional) The selectable to select when this scene is loaded. Make sure the GameObject placed here has a selectable component.")]
    public GameObject SelectOnStartup;

    private void Start()
    {
        if (SelectOnStartup != null)
        {
            SelectOnStartup.GetComponent<Selectable>().Select();
        }
    }
    
    // Set GameStats's NumOfPlayers variable based on the number of players ready.
    // This should be executed once player select has finished.
    public void SetNumOfPlayers()
    {
        int pNum = 0;

        foreach(bool p in GameStats.PlayersReady)
        {
            if (p) pNum++;
        }

        GameStats.NumOfPlayers = pNum;
    }

    // Resets the player variables in GameStats. Executed when someone resets the game.
    public void ResetGameStatsPlayers()
    {
        GameStats.NumOfPlayers = 1;
        GameStats.PlayersReady = new bool[] { false, false, false, false };
        GameStats.PlayersFinished = 0;
    }

    // Load a scene, duh.
    public void LoadScene(string sceneString)
    {
        SceneManager.LoadScene(sceneString);
    }

    // Closes the game.
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Application Closed");
    }
}
