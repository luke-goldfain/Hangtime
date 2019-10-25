using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupManager : MonoBehaviour
{
    
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

    // Load a scene, duh.
    public void LoadScene(string sceneString)
    {
        SceneManager.LoadScene(sceneString);
    }

    //Closes the game if button is pressed.
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
