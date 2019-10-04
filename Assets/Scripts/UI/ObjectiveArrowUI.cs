using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ObjectiveArrowUI : MonoBehaviour
{

    public GameObject curobjec;
    public Image img;

    public GameObject ObjectiveSystem;

    private List<Transform> targetListReference;
    private Transform playerReference;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetListReference = ObjectiveSystem.GetComponent<TargetManager>().TargetList;
        playerReference = ObjectiveSystem.GetComponent<TargetManager>().playerReference;

        Pointer();
    }

    void Pointer()
    {
        // TODO: Make the arrow point on its x,y plane relative to the direction of targetList[0] from the player,
        // accounting for player's transform.rotation.

        img = GetComponent<Image>();
        img.enabled = false;

        if (targetListReference.Count > 0)
        {
            curobjec = targetListReference[0].gameObject;
        }

        if (curobjec != null)
        {
            Vector3 v3Pos = Camera.main.WorldToViewportPoint(curobjec.transform.position);

            if (v3Pos.z < Camera.main.nearClipPlane)
                return;

            if (v3Pos.x >= 0.0f && v3Pos.x <= 1.0f && v3Pos.y >= 0.0f && v3Pos.y <= 1.0f)
                return;


            img.enabled = true;
            v3Pos.x -= 0.5f;
            v3Pos.y -= 0.5f;
            v3Pos.z = 0;

            float fAngle = Mathf.Atan2(v3Pos.x, v3Pos.y);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fAngle * Mathf.Rad2Deg);

            v3Pos.x = 0.5f * Mathf.Sin(fAngle) + 0.5f;
            v3Pos.y = 0.5f * Mathf.Cos(fAngle) + 0.5f;
            v3Pos.z = Camera.main.nearClipPlane + 0.01f;
            transform.position = Camera.main.ViewportToWorldPoint(v3Pos);
        }


    }
}
