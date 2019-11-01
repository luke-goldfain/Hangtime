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

    public List<ListWrapper> RndCheckpointLists { get; private set; }

    private int listChoice;

    private List<Vector3> ChosenCheckpointList;
    public int CurrentCheckptTarget;

    private float currentHeight;
    private float prevHeight;

    private Rigidbody rb;

    public GameObject CheckpointPrefab;
    public GameObject FinishZonePrefab;

    public LayerMask CheckpointMask;

    private bool finishPlaced;

    // Start
    void Start()
    {
        RndCheckpointLists = this.gameObject.GetComponent<CheckpointLists>().CheckpointListsList;

        listChoice = UnityEngine.Random.Range(0, RndCheckpointLists.Count);

        Debug.Log("listChoice: " + listChoice);

        ChosenCheckpointList = RndCheckpointLists[listChoice].Vector3List;

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

        if (Vector3.Distance(this.transform.position, ChosenCheckpointList[CurrentCheckptTarget]) <= 40f &&
            Vector3.Distance(this.transform.position, ChosenCheckpointList[CurrentCheckptTarget]) > 2f)
        {
            speedToLerp = minSpeed;
        }

        if (Vector3.Distance(this.transform.position, ChosenCheckpointList[CurrentCheckptTarget]) <= 2f)
        {
            if (ChosenCheckpointList.Count > CurrentCheckptTarget + 1)
            {
                DropCheckpoint();

                AkSoundEngine.PostEvent("CheckpointDrop", GameObject.Find("Main Camera"));

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



    // This function basically raycasts downwards from four points in the cardinal directions from the follow orb.
    // If the orb detects that it's moving uphill, it will correct to a direction wherein the raycast down returns a 
    // higher distance, meaning the orb will trend towards a higher altitude.
    private void UpdateMovePreferringHighAltitude()
    {
        Vector3 moveTarget = ChosenCheckpointList[CurrentCheckptTarget];

        prevHeight = currentHeight;

        if (Physics.Raycast(this.transform.position + (Vector3.down * 4), Vector3.down, out RaycastHit hit, Mathf.Infinity, CheckpointMask))
        {
            currentHeight = Vector3.Distance(this.transform.position, hit.point);

            if (currentHeight < prevHeight)
            {
                Vector3 tallestHeight = ChosenCheckpointList[CurrentCheckptTarget]; // Default to continuing to move to checkpoint
                float tempCurrentHeight = currentHeight;

                if (Physics.Raycast(this.transform.position + (Vector3.left * 4) + (Vector3.down * 4), Vector3.down, out RaycastHit leftHit, Mathf.Infinity, CheckpointMask))
                {
                    if (Vector3.Distance(this.transform.position + (Vector3.left * 4) + (Vector3.down * 4), leftHit.point) > tempCurrentHeight &&
                        Vector3.Distance(this.transform.position + (Vector3.left * 4), ChosenCheckpointList[CurrentCheckptTarget]) < Vector3.Distance(this.transform.position, ChosenCheckpointList[CurrentCheckptTarget]))
                    {
                        tallestHeight = this.transform.position + (Vector3.left * 4);

                        tempCurrentHeight = Vector3.Distance(tallestHeight, leftHit.point);
                    }
                }

                if (Physics.Raycast(this.transform.position + (Vector3.right * 4) + (Vector3.down * 4), Vector3.down, out RaycastHit rightHit, Mathf.Infinity, CheckpointMask))
                {
                    if (Vector3.Distance(this.transform.position + (Vector3.right * 4) + (Vector3.down * 4), rightHit.point) > tempCurrentHeight &&
                        Vector3.Distance(this.transform.position + (Vector3.right * 4), ChosenCheckpointList[CurrentCheckptTarget]) < Vector3.Distance(this.transform.position, ChosenCheckpointList[CurrentCheckptTarget]))
                    {
                        tallestHeight = this.transform.position + (Vector3.right * 4);

                        tempCurrentHeight = Vector3.Distance(tallestHeight, rightHit.point);
                    }
                }

                if (Physics.Raycast(this.transform.position + (Vector3.forward * 4) + (Vector3.down * 4), Vector3.down, out RaycastHit forwardHit, Mathf.Infinity, CheckpointMask))
                {
                    if (Vector3.Distance(this.transform.position + (Vector3.forward * 4) + (Vector3.down * 4), forwardHit.point) > tempCurrentHeight &&
                        Vector3.Distance(this.transform.position + (Vector3.forward * 4), ChosenCheckpointList[CurrentCheckptTarget]) < Vector3.Distance(this.transform.position, ChosenCheckpointList[CurrentCheckptTarget]))
                    {
                        tallestHeight = this.transform.position + (Vector3.forward * 4);

                        tempCurrentHeight = Vector3.Distance(tallestHeight, forwardHit.point);
                    }
                }

                if (Physics.Raycast(this.transform.position + (Vector3.back * 4) + (Vector3.down * 4), Vector3.down, out RaycastHit backHit, Mathf.Infinity, CheckpointMask))
                {
                    if (Vector3.Distance(this.transform.position + (Vector3.back * 4) + (Vector3.down * 4), backHit.point) > tempCurrentHeight &&
                        Vector3.Distance(this.transform.position + (Vector3.back * 4), ChosenCheckpointList[CurrentCheckptTarget]) < Vector3.Distance(this.transform.position, ChosenCheckpointList[CurrentCheckptTarget]))
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
            GameObject cp = Instantiate(CheckpointPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

            // Reset the rotation of the beacons
            foreach(Transform child in cp.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<ParticleSystem>() != null)
                {
                    child.rotation = Quaternion.identity;
                }
            }
        }
    }

    private void DropFinishZone()
    {

        if (Physics.Raycast(this.transform.position + (Vector3.down * 4), Vector3.down, out RaycastHit hit, Mathf.Infinity, CheckpointMask))
        {
            Instantiate(FinishZonePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        }
    }
}
