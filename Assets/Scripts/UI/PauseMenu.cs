using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public static bool ispaused = false;
    public Canvas UI;
    public Canvas PauseUI;
    // Start is called before the first frame update
    void Start()
    {
        PauseUI.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ispaused)
            {
                Resume();

                AkSoundEngine.PostEvent("Deselect", gameObject);
            }
            else
            {
                Pause();

                AkSoundEngine.PostEvent("Select", gameObject);
            }
        }
    }

    public void Resume ()
    {
        PauseUI.enabled = false;
        UI.enabled = true;
        Time.timeScale = 0;
        ispaused = true;
    }

    void Pause()
    {
        Cursor.visible = true;
        PauseUI.enabled = true;
        UI.enabled = false;
        Time.timeScale = 0;
        ispaused = true;
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
        ispaused = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
