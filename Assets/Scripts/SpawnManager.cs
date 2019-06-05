using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    public GameObject PlayerPrefab, ReticlePrefab, SpeedometerPrefab;

    private List<Vector3> InitialSpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        InitialSpawnPoints = new List<Vector3>();

        InitialSpawnPoints.Add(new Vector3(0, 2, 0));
        InitialSpawnPoints.Add(new Vector3(5, 2, 0));
        InitialSpawnPoints.Add(new Vector3(5, 2, -5));
        InitialSpawnPoints.Add(new Vector3(0, 2, -5));

        for (int i = 1; i <= GameStats.NumOfPlayers; i++)
        {
            GameObject currentPlayer = Instantiate(PlayerPrefab, InitialSpawnPoints[i - 1], Quaternion.identity);
            currentPlayer.GetComponent<PlayerController>().PlayerNumber = i;
            currentPlayer.GetComponent<PlayerController>().Reticle = Instantiate(ReticlePrefab, FindObjectOfType<Canvas>().transform);
            currentPlayer.GetComponent<PlayerController>().Speedometer = Instantiate(SpeedometerPrefab, FindObjectOfType<Canvas>().transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
