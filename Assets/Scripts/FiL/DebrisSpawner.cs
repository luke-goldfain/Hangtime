using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    public GameObject[] Debris;
    public GameObject curspawn;
    int index;
    public GameObject mid_Deb;
    public GameObject lrg_Deb;
    private int ab;
    private int RandX;
    private int RandZ;

    private float nextActionTime = 0.0f;
    public float period = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnDebris", 2.0f, .2f);
        
    }

    // Update is called once per frame
    void Update()
    {


    }

    void SpawnDebris()
    {

        ab = Random.Range(0, 2);
        RandX = Random.Range(-120, 120);
        RandZ = Random.Range(-120, 120);

        if (ab == 1)
        {
            Instantiate(mid_Deb, new Vector3(RandX, 40, RandZ), Quaternion.identity);
        }

        if (ab == 2)
        {
            Instantiate(mid_Deb, new Vector3(RandX, 40, RandZ), Quaternion.identity);
        }
    }
}
