using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupManager : MonoBehaviour
{
    public void LoadScene(string sceneString)
    {
        SceneManager.LoadScene(sceneString);
    }
}
