using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupManager : MonoBehaviour
{
    public void SetNumberOfPlayers(int pNum)
    {
        GameStats.NumOfPlayers = pNum;
    }

    public void LoadScene(string sceneString)
    {
        SceneManager.LoadScene(sceneString);
    }
}
