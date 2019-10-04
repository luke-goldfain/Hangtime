using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedConverter : MonoBehaviour
{

    static float minAngle = 0.0f;
    static float maxAngle = -203.2f;

    // Start is called before the first frame update
    void Start()
    {


    }
    
    public void ShowSpeed(float speed, float min, float max)
    {
        float ang = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(min, max, speed));
        this.gameObject.transform.eulerAngles = new Vector3(0, 0, ang);
    }
}
