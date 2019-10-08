using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class TargetManager : MonoBehaviour
{
    [Tooltip("Reference to the player that this indicator belongs to.")]
    public Transform playerReference;
    [Tooltip("Rference to the indicator itself.")]
    public Image indicatorReference;

    // List of targets for the indicator to select from.
    public List<Transform> TargetList { get; private set; }

    private Vector3 indicatorVector; // unused. not sure what this did
    private Text textReference; // reference to the text of the indicator, used to display distance in meters of player from target
    private int targetRange; // (now obsolete) the range within which the target will mark as "reached" and move on to next
    private int targetIndex; // the index in TargetList of the currently marked target 
    private bool targetValidated; // whether there are any targets available (until orb sets a checkpoint, this will be false)

    private float minXPos, minYPos, maxXPos, maxYPos;


    void Awake()
    {
        indicatorReference = this.GetComponentInChildren<Image>();

        TargetList = new List<Transform>();

        textReference = indicatorReference.GetComponentInChildren<Text>();
    }

    void Start()
    {
        StartSetIndicatorClamps();

        targetIndex = 0;
        targetRange = 5;
        
        targetValidated = TargetList.Count > 0;
    }

    private void Update()
    {
        // Correct each frame for new checkpoints that are not yet in the target list
        if (TargetList.Count < playerReference.GetComponent<CheckpointController>().CheckpointsTotalPlaced.Length - playerReference.GetComponent<CheckpointController>().CheckpointsHit.Count)
        {
            foreach (GameObject cp in playerReference.GetComponent<CheckpointController>().CheckpointsTotalPlaced)
            {
                if (!TargetList.Contains(cp.transform))
                {
                    TargetList.Add(cp.transform);
                }
            }
        }

        // Correct each frame for checkpoints that have just been hit by player
        if (TargetList.Count > playerReference.GetComponent<CheckpointController>().CheckpointsTotalPlaced.Length - playerReference.GetComponent<CheckpointController>().CheckpointsHit.Count)
        {
            foreach (GameObject cp in playerReference.GetComponent<CheckpointController>().CheckpointsTotalPlaced)
            {
                if (playerReference.GetComponent<CheckpointController>().CheckpointsHit.Contains(cp))
                {
                    TargetList.Remove(cp.transform);
                }
            }
        }

        targetValidated = TargetList.Count > 0;
    }

    void LateUpdate()
    {
        if (targetValidated)
        {
            //if (targetList[targetIndex])
            //{
            //    targetIndex = (targetIndex + 1) % targetList.Count;
            //}
        }
        
        UpdateTargetSystem(targetIndex);
    }

    // Sets the clamps that the indicator will stay between, so that it doesn't move off the player's screen.
    private void StartSetIndicatorClamps()
    {
        Camera pCam = playerReference.GetComponentInChildren<Camera>();
        int numOfPlayers = GameStats.NumOfPlayers;

        float xBorder = pCam.pixelWidth * 0.05f;
        float yBorder = pCam.pixelHeight * 0.05f;

        switch (playerReference.GetComponent<PlayerController>().PlayerNumber) //TODO
        {
            case 1:
                minXPos = xBorder;

                if (numOfPlayers < 3) minYPos = yBorder;
                else minYPos = (Screen.height / 2) + yBorder;

                if (numOfPlayers < 2) maxXPos = (pCam.pixelWidth) - xBorder;
                else maxXPos = (Screen.width / 2) - xBorder;

                maxYPos = Screen.height - yBorder;
                break;
            case 2:
                minXPos = (Screen.width / 2) + xBorder;

                if (numOfPlayers < 3) minYPos = yBorder;
                else minYPos = (Screen.height / 2) + yBorder;

                maxXPos = Screen.width - xBorder;

                maxYPos = Screen.height - yBorder;
                break;
            case 3:
                minXPos = xBorder;
                minYPos = yBorder;
                maxXPos = (pCam.pixelWidth) - xBorder;
                maxYPos = (pCam.pixelHeight) - yBorder;
                break;
            case 4:
                minXPos = (Screen.width / 2) + xBorder;
                minYPos = yBorder;
                maxXPos = (Screen.width) - xBorder;
                maxYPos = (Screen.height / 2) - yBorder;
                break;
        }
    }

    public void UpdateTargetSystem(int index)
    {
        if (targetValidated)
        {
          
            //indicatorReference.gameObject.SetActive(RelativePosition(playerReference, targetList[index]));
            indicatorReference.gameObject.SetActive(true);
            
            if (TargetList[index].gameObject.activeInHierarchy)
            {
               
                textReference.text = LinearDistance(playerReference.position, TargetList[index].position) + "m";

                // Heading variable prevents checkpoint marker to appear in the opposite direction.
                // This is a workaround for Unity's WolrdToScreenPoint silliness.
                Vector3 heading = TargetList[index].transform.position - playerReference.transform.position;

                // If the player is facing towards the checkpoint, display the indicator on their screen.
                if (Vector3.Dot(playerReference.GetComponentInChildren<Camera>().transform.forward, heading) > 0)
                {
                    indicatorReference.transform.position = playerReference.GetComponentInChildren<Camera>().WorldToScreenPoint(TargetList[index].position + (Vector3.up * 10f));

                    // Clamp indicator position to screen.
                    indicatorReference.transform.position = new Vector3(Mathf.Clamp(indicatorReference.transform.position.x, minXPos, maxXPos), 
                                                                        Mathf.Clamp(indicatorReference.transform.position.y, minYPos, maxYPos));
                }
                else
                {
                    indicatorReference.gameObject.SetActive(false);
                }

                //indicatorVector = indicatorReference.rectTransform.anchorMin;
                //indicatorVector.x = Camera.main.WorldToViewportPoint(targetList[index].position).x;
                //indicatorReference.rectTransform.anchorMin = indicatorVector;
                //indicatorReference.rectTransform.anchorMax = indicatorVector;
            }
        }
        else
        {
            indicatorReference.gameObject.SetActive(false);
        }
    }
   
    public int LinearDistance(Vector3 playerPosition, Vector3 targetPosition)
    {
        
        playerPosition.y = 0;
        targetPosition.y = 0;
        
        return Mathf.RoundToInt(Vector3.Distance(playerPosition, targetPosition));
    }
   
    private bool RelativePosition(Transform player, Transform target)
    {
        
        return Vector3.Dot(Vector3.forward, player.InverseTransformPoint(target.position).normalized) > 0;
    }
}
