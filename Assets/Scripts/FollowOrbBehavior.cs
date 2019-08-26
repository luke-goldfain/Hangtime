using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOrbBehavior : MonoBehaviour
{
    public float maxSpeed;
    public float minSpeed;
    private float speed;
    private float speedToLerp;

    public List<Vector3> CheckpointPositions;
    public int CurrentCheckptTarget;

    private float currentHeight;
    private float prevHeight;

    private Rigidbody rb;

    public GameObject CheckpointPrefab;
    public GameObject FinishZonePrefab;

    public LayerMask CheckpointMask;

    private bool finishPlaced;

    // Start is called before the first frame update
    void Start()
    {
        finishPlaced = false;

        rb = GetComponent<Rigidbody>();

        speed = minSpeed;
        speedToLerp = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        speed = Mathf.Lerp(speed, speedToLerp, 0.1f);

        UpdateMovePreferringHighAltitude();

        if (Vector3.Distance(this.transform.position, CheckpointPositions[CurrentCheckptTarget]) <= 40f &&
            Vector3.Distance(this.transform.position, CheckpointPositions[CurrentCheckptTarget]) > 2f)
        {
            speedToLerp = minSpeed;
        }

        if (Vector3.Distance(this.transform.position, CheckpointPositions[CurrentCheckptTarget]) <= 2f)
        {
            if (CheckpointPositions.Count > CurrentCheckptTarget + 1)
            {
                DropCheckpoint();

                CurrentCheckptTarget++;
            }
            else if (!finishPlaced)
            {
                DropFinishZone();

                finishPlaced = true;
            }
            
            speedToLerp = maxSpeed;
        }
    }

    private void UpdateMovePreferringHighAltitude()
    {
        Vector3 moveTarget = CheckpointPositions[CurrentCheckptTarget];

        prevHeight = currentHeight;

        // Dear god. So this basically raycasts downwards from four points in the cardinal directions from the follow orb.
        // If the orb detects that it's moving uphill, it will correct to a direction wherein the raycast down returns a 
        // higher distance, meaning the orb will trend towards a higher altitude.
        if (Physics.Raycast(this.transform.position + (Vector3.down * 4), Vector3.down, out RaycastHit hit, Mathf.Infinity, CheckpointMask))
        {
            currentHeight = Vector3.Distance(this.transform.position, hit.point);

            if (currentHeight < prevHeight)
            {
                Vector3 tallestHeight = CheckpointPositions[CurrentCheckptTarget]; // Default to continuing to move to checkpoint
                float tempCurrentHeight = currentHeight;

                if (Physics.Raycast(this.transform.position + (Vector3.left * 4) + (Vector3.down * 4), Vector3.down, out RaycastHit leftHit, Mathf.Infinity, CheckpointMask))
                {
                    if (Vector3.Distance(this.transform.position + (Vector3.left * 4) + (Vector3.down * 4), leftHit.point) > tempCurrentHeight &&
                        Vector3.Distance(this.transform.position + (Vector3.left * 4), CheckpointPositions[CurrentCheckptTarget]) < Vector3.Distance(this.transform.position, CheckpointPositions[CurrentCheckptTarget]))
                    {
                        tallestHeight = this.transform.position + (Vector3.left * 4);

                        tempCurrentHeight = Vector3.Distance(tallestHeight, leftHit.point);
                    }
                }

                if (Physics.Raycast(this.transform.position + (Vector3.right * 4) + (Vector3.down * 4), Vector3.down, out RaycastHit rightHit, Mathf.Infinity, CheckpointMask))
                {
                    if (Vector3.Distance(this.transform.position + (Vector3.right * 4) + (Vector3.down * 4), rightHit.point) > tempCurrentHeight &&
                        Vector3.Distance(this.transform.position + (Vector3.right * 4), CheckpointPositions[CurrentCheckptTarget]) < Vector3.Distance(this.transform.position, CheckpointPositions[CurrentCheckptTarget]))
                    {
                        tallestHeight = this.transform.position + (Vector3.right * 4);

                        tempCurrentHeight = Vector3.Distance(tallestHeight, rightHit.point);
                    }
                }

                if (Physics.Raycast(this.transform.position + (Vector3.forward * 4) + (Vector3.down * 4), Vector3.down, out RaycastHit forwardHit, Mathf.Infinity, CheckpointMask))
                {
                    if (Vector3.Distance(this.transform.position + (Vector3.forward * 4) + (Vector3.down * 4), forwardHit.point) > tempCurrentHeight &&
                        Vector3.Distance(this.transform.position + (Vector3.forward * 4), CheckpointPositions[CurrentCheckptTarget]) < Vector3.Distance(this.transform.position, CheckpointPositions[CurrentCheckptTarget]))
                    {
                        tallestHeight = this.transform.position + (Vector3.forward * 4);

                        tempCurrentHeight = Vector3.Distance(tallestHeight, forwardHit.point);
                    }
                }

                if (Physics.Raycast(this.transform.position + (Vector3.back * 4) + (Vector3.down * 4), Vector3.down, out RaycastHit backHit, Mathf.Infinity, CheckpointMask))
                {
                    if (Vector3.Distance(this.transform.position + (Vector3.back * 4) + (Vector3.down * 4), backHit.point) > tempCurrentHeight &&
                        Vector3.Distance(this.transform.position + (Vector3.back * 4), CheckpointPositions[CurrentCheckptTarget]) < Vector3.Distance(this.transform.position, CheckpointPositions[CurrentCheckptTarget]))
                    {
                        tallestHeight = this.transform.position + (Vector3.back * 4);

                        tempCurrentHeight = Vector3.Distance(tallestHeight, backHit.point);
                    }
                }

                moveTarget = tallestHeight;
            }
        }

        this.transform.position = Vector3.MoveTowards(this.transform.position, moveTarget, speed);
    }

    private void DropCheckpoint()
    {

        if (Physics.Raycast(this.transform.position + (Vector3.down * 4), Vector3.down, out RaycastHit hit, Mathf.Infinity, CheckpointMask))
        {
            Instantiate(CheckpointPrefab, hit.point, Quaternion.identity);
        }
    }

    private void DropFinishZone()
    {

        if (Physics.Raycast(this.transform.position + (Vector3.down * 4), Vector3.down, out RaycastHit hit, Mathf.Infinity, CheckpointMask))
        {
            Instantiate(FinishZonePrefab, hit.point, Quaternion.identity);
        }
    }
}
