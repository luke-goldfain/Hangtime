using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSectionPositioner : MonoBehaviour
{
    private Vector3 playerPos, grapplePos;
    public int SectionNumber;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = GetComponentInParent<PlayerController>().transform.position;

        grapplePos = GetComponentInParent<PlayerController>().GrappleHitPosition;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = GetComponentInParent<PlayerController>().transform.position;
        
        grapplePos = GetComponentInParent<PlayerController>().GrappleHitPosition;
        
        Vector3 avgPosition = (grapplePos + playerPos) / 2;
        Vector3 avgPSide = (avgPosition + playerPos) / 2;
        Vector3 avgGSide = (avgPosition + grapplePos) / 2;

        switch (SectionNumber % 3)
        {
            case 0:
                this.transform.position = avgPosition;
                break;
            case 1:
                this.transform.position = avgPSide;
                break;
            case 2:
                this.transform.position = avgGSide;
                break;
        }
    }
}
