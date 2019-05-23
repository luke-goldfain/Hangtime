using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerReference : MonoBehaviour
{
    public GameObject Player;

    private int speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speed = Mathf.RoundToInt(Player.GetComponent<Rigidbody>().velocity.magnitude);

        this.GetComponent<Text>().text = speed.ToString();
    }
}
