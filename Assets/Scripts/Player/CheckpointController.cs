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

    public List<GameObject> CheckpointsHit { get; private set; }

    private bool finishable;
    public bool Finished { get; private set; }

    private GameObject CheckpointMeterFill;

    // Start is called before the first frame update
    void Start()
    {
        playerNumber = this.gameObject.GetComponent<PlayerController>().PlayerNumber;

        CheckpointsHit = new List<GameObject>();

        finishable = false;

        Finished = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<PlayerController>().CheckpointText.GetComponent<TextMeshProUGUI>().text = CheckpointsHit.Count + " / " + CheckpointsRequired;

        CheckpointMeterFill = this.gameObject.GetComponent<PlayerController>().CheckpointMeterFill;

        if (CheckpointsHit.Count > 0)
        {
            CheckpointMeterFill.GetComponent<Image>().fillAmount = (float)CheckpointsHit.Count / CheckpointsRequired;
        }

        if (CheckpointsHit.Count >= CheckpointsRequired)
        {
            finishable = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            if (!CheckpointsHit.Contains(other.gameObject))
            {
                CheckpointsHit.Add(other.gameObject);
            }

            MeshRenderer[] meshes = other.gameObject.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer pv in meshes)
            {
                if (pv.gameObject.layer == LayerMask.NameToLayer("P" + playerNumber + "View"))
                {
                    pv.gameObject.SetActive(false);
                }
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
