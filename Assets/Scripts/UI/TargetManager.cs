using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public Transform playerReference;
    public Image indicatorReference;
    public List<Transform> targetList;

    private Vector3 indicatorVector;
    private Text textReference;
    private int targetRange;
    private int targetIndex;
    private bool targetValidated;


    void Awake()
    {
        
        textReference = indicatorReference.GetComponentInChildren<Text>();
    }

    void Start()
    {
        
        targetIndex = 0;
        targetRange = 5;
        
        targetValidated = targetList.Count > 0;
    }

    void LateUpdate()
    {
        
        if (targetValidated)
        {
            if (LinearDistance(playerReference.position, targetList[targetIndex].position) < targetRange)
            {
                targetIndex = (targetIndex + 1) % targetList.Count;
            }
           
            UpdateTargetSystem(targetIndex);
        }
    }

    public void UpdateTargetSystem(int index)
    {
        if (targetValidated)
        {
          
            indicatorReference.gameObject.SetActive(RelativePosition(playerReference, targetList[index]));
            
            if (targetList[index].gameObject.activeInHierarchy)
            {
               
                textReference.text = LinearDistance(playerReference.position, targetList[index].position) + "m";

                // Heading variable prevents checkpoint marker to appear in the opposite direction.
                // This is a workaround for Unity's WolrdToScreenPoint silliness.
                Vector3 heading = targetList[index].transform.position - playerReference.transform.position;

                // If the player is facing towards the checkpoint, display the indicator on their screen.
                if (Vector3.Dot(playerReference.GetComponentInChildren<Camera>().transform.forward, heading) > 0)
                {
                    indicatorReference.transform.position = playerReference.GetComponentInChildren<Camera>().WorldToScreenPoint(targetList[index].position + (Vector3.up * 10f));
                }

                //indicatorVector = indicatorReference.rectTransform.anchorMin;
                //indicatorVector.x = Camera.main.WorldToViewportPoint(targetList[index].position).x;
                //indicatorReference.rectTransform.anchorMin = indicatorVector;
                //indicatorReference.rectTransform.anchorMax = indicatorVector;
            }
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
