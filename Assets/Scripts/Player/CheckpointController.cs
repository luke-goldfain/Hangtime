using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointController : MonoBehaviour
{
    public int CheckpointsRequired;

    private int playerNumber;

    public GameObject[] CheckpointsTotalPlaced { get; private set; }

    public List<GameObject> CheckpointsHit { get; private set; }

    public bool Finishable { get; private set; }
    public bool Finished { get; private set; }

    private GameObject CheckpointMeterFill;

    // Start is called before the first frame update
    void Start()
    {
        playerNumber = this.gameObject.GetComponent<PlayerController>().PlayerNumber;

        CheckpointsHit = new List<GameObject>();

        Finishable = false;

        Finished = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckpointsTotalPlaced = GameObject.FindGameObjectsWithTag("Checkpoint");

        if (Finishable)
        {
            CheckpointsTotalPlaced = GameObject.FindGameObjectsWithTag("Finish");
        }

        this.gameObject.GetComponent<PlayerController>().CheckpointText.GetComponent<TextMeshProUGUI>().text = CheckpointsHit.Count + " / " + CheckpointsRequired;

        CheckpointMeterFill = this.gameObject.GetComponent<PlayerController>().CheckpointMeterFill;

        if (CheckpointsHit.Count > 0)
        {
            CheckpointMeterFill.GetComponent<Image>().fillAmount = (float)CheckpointsHit.Count / CheckpointsRequired;
        }

        if (CheckpointsHit.Count >= CheckpointsRequired)
        {
            Finishable = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            if (!CheckpointsHit.Contains(other.gameObject))
            {
                CheckpointsHit.Add(other.gameObject);

                AkSoundEngine.PostEvent("Checkpoint", GameObject.Find("Main Camera"));
            }

            Transform[] cpChildren = other.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform pv in cpChildren)
            {
                if (pv.gameObject.layer == LayerMask.NameToLayer("P" + playerNumber + "View"))
                {
                    pv.gameObject.SetActive(false);
                }
            }
        }

        if (other.tag == "Finish")
        {
            if (Finishable)
            {
                RaceFinish();

                AkSoundEngine.PostEvent("Finish", GameObject.Find("Main Camera"));
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
