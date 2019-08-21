using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOrbBehavior : MonoBehaviour
{
    public List<Vector3> CheckpointPositions;
    private int currentCheckptTarget;

    public GameObject CheckpointPrefab;
    public GameObject FinishZonePrefab;

    public LayerMask CheckpointMask;

    private bool finishPlaced;

    // Start is called before the first frame update
    void Start()
    {
        finishPlaced = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, CheckpointPositions[currentCheckptTarget], 0.01f);

        if (Vector3.Distance(this.transform.position, CheckpointPositions[currentCheckptTarget]) <= 5f)
        {
            if (CheckpointPositions.Count > currentCheckptTarget + 1)
            {
                DropCheckpoint();

                currentCheckptTarget++;
            }
            else if (!finishPlaced)
            {
                DropFinishZone();

                finishPlaced = true;
            }
        }
    }

    private void DropCheckpoint()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position + (Vector3.down * 4), Vector3.down, out hit, Mathf.Infinity, CheckpointMask))
        {
            Instantiate(CheckpointPrefab, hit.point, Quaternion.identity);
        }
    }

    private void DropFinishZone()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position + (Vector3.down * 4), Vector3.down, out hit, Mathf.Infinity, CheckpointMask))
        {
            Instantiate(FinishZonePrefab, hit.point, Quaternion.identity);
        }
    }
}
