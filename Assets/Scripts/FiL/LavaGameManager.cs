using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LavaGameManager : MonoBehaviour
{
    public Text gameover;
    public bool losestate = false;
    private int time;
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        gameover.enabled = false;
        gameover.text = "GAME OVER. Press E to play again";
    }

    // Update is called once per frame
    void Update()
    {
        time++;
        if (time >= 60)
        {
            score++;
            time = 0;
        }

        if (gameover == true)
        {
            if (Input.GetKeyDown("e"))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public void OnTriggerEnter(Collider Lava)
    {
        Time.timeScale = 0.0f;
        gameover.enabled = true;

    }
}
