using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public int CheckpointsRequired;

    private List<GameObject> checkpointsHit;

    private bool finishable;

    // Start is called before the first frame update
    void Start()
    {
        checkpointsHit = new List<GameObject>();

        finishable = false;
    }

    // Update is called once per frame
    void Update()
    {
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
        this.gameObject.GetComponent<PlayerController>().AcceptsInput = false;
        
        // TODO: Add a "finished" state that zooms camera out and displays player character in third person
    }
}
