using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public Transform playerReference;
    public Image indicatorReference;
    public List<Transform> targetList { get; private set; }

    private Vector3 indicatorVector;
    private Text textReference;
    private int targetRange;
    private int targetIndex;
    private bool targetValidated;


    void Awake()
    {
        indicatorReference = this.GetComponentInChildren<Image>();

        targetList = new List<Transform>();

        textReference = indicatorReference.GetComponentInChildren<Text>();
    }

    void Start()
    {
        
        targetIndex = 0;
        targetRange = 5;
        
        targetValidated = targetList.Count > 0;
    }

    private void Update()
    {
        // Correct each frame for new checkpoints that are not yet in the target list
        if (targetList.Count < playerReference.GetComponent<CheckpointController>().CheckpointsTotalPlaced.Length - playerReference.GetComponent<CheckpointController>().CheckpointsHit.Count)
        {
            foreach (GameObject cp in playerReference.GetComponent<CheckpointController>().CheckpointsTotalPlaced)
            {
                if (!targetList.Contains(cp.transform))
                {
                    targetList.Add(cp.transform);
                }
            }
        }

        // Correct each frame for checkpoints that have just been hit by player
        if (targetList.Count > playerReference.GetComponent<CheckpointController>().CheckpointsTotalPlaced.Length - playerReference.GetComponent<CheckpointController>().CheckpointsHit.Count)
        {
            foreach (GameObject cp in playerReference.GetComponent<CheckpointController>().CheckpointsTotalPlaced)
            {
                if (playerReference.GetComponent<CheckpointController>().CheckpointsHit.Contains(cp))
                {
                    targetList.Remove(cp.transform);
                }
            }
        }

        targetValidated = targetList.Count > 0;
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

    public void UpdateTargetSystem(int index)
    {
        if (targetValidated)
        {
          
            //indicatorReference.gameObject.SetActive(RelativePosition(playerReference, targetList[index]));
            indicatorReference.gameObject.SetActive(true);
            
            if (targetList[index].gameObject.activeInHierarchy)
            {
               
                textReference.text = LinearDistance(playerReference.position, targetList[index].position) + "m";

                indicatorReference.transform.position = playerReference.GetComponentInChildren<Camera>().WorldToScreenPoint(targetList[index].position + (Vector3.up * 10f));

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
