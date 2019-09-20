using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointController : MonoBehaviour
{
    public int CheckpointsRequired;

    private List<GameObject> checkpointsHit;

    private bool finishable;
    public bool Finished { get; private set; }

    private GameObject CheckpointMeterFill;

    // Start is called before the first frame update
    void Start()
    {
        checkpointsHit = new List<GameObject>();

        finishable = false;

        Finished = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<PlayerController>().CheckpointText.GetComponent<TextMeshProUGUI>().text = checkpointsHit.Count + " / " + CheckpointsRequired;

        CheckpointMeterFill = this.gameObject.GetComponent<PlayerController>().CheckpointMeterFill;

        if (checkpointsHit.Count > 0)
        {
            CheckpointMeterFill.GetComponent<Image>().fillAmount = (float)checkpointsHit.Count / CheckpointsRequired;
        }

        if (checkpointsHit.Count >= CheckpointsRequired)
        {
            finishable = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            if (!checkpointsHit.Contains(other.gameObject))
            {
                checkpointsHit.Add(other.gameObject);
            }
        }

        if (other.tag == "Finish")
        {
            if (finishable)
            {
                RaceFinish();
            }
        }
    }

    private void RaceFinish()
    {
        if (!Finished)
        {
            this.gameObject.GetComponent<PlayerController>().AcceptsInput = false;

            this.gameObject.GetComponent<PlayerController>().PlacementText.SetActive(true);

            switch (GameStats.PlayersFinished)
            {
                case 0:
                    this.gameObject.GetComponent<PlayerController>().PlacementText.GetComponent<TextMeshProUGUI>().text = "FIRST PLACE!";
                    break;
                case 1:
                    this.gameObject.GetComponent<PlayerController>().PlacementText.GetComponent<TextMeshProUGUI>().text = "SECOND PLACE!";
                    break;
                case 2:
                    this.gameObject.GetComponent<PlayerController>().PlacementText.GetComponent<TextMeshProUGUI>().text = "THIRD PLACE!";
                    break;
                case 3:
                    this.gameObject.GetComponent<PlayerController>().PlacementText.GetComponent<TextMeshProUGUI>().text = "FOURTH PLACE!";
                    break;
            }

            GameStats.PlayersFinished++;

            Finished = true;
        }
        
        // TODO: Add a "finished" state that zooms camera out and displays player character in third person
    }
}
