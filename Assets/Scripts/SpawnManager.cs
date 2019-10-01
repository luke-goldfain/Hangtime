using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    public GameObject PlayerPrefab, HUDPrefab;
    //public GameObject PlayerPrefab, ReticlePrefab, SpeedometerPrefab, CheckpointMeterPrefab, CheckpointMeterFillPrefab;

    [SerializeField, Tooltip("The initial spawn points of each player in the current scene.")]
    public List<Vector3> InitialSpawnPoints = new List<Vector3>();

    [SerializeField, Tooltip("(Unfunctional) The Y rotation in degrees of the spawn point.")]
    public float SpawnRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (InitialSpawnPoints.Count < 4)
        {
            InitialSpawnPoints.Add(new Vector3(1 * InitialSpawnPoints.Count, 2, 0));
        }

        for (int i = 1; i <= GameStats.PlayersReady.Length; i++)
        {
            if (GameStats.PlayersReady[i - 1] == true)
            {
                GameObject currentPlayer = Instantiate(PlayerPrefab, InitialSpawnPoints[i - 1], Quaternion.identity);
                currentPlayer.transform.Rotate(0f, SpawnRotation, 0f); // DOESN'T WORK >:(
                currentPlayer.GetComponent<PlayerController>().PlayerNumber = i;

                currentPlayer.GetComponent<PlayerController>().HUD = Instantiate(HUDPrefab, Vector3.zero, Quaternion.identity);
            }

            /*currentPlayer.GetComponent<PlayerController>().Reticle = Instantiate(ReticlePrefab, FindObjectOfType<Canvas>().transform);
            currentPlayer.GetComponent<PlayerController>().Speedometer = Instantiate(SpeedometerPrefab, FindObjectOfType<Canvas>().transform);
            currentPlayer.GetComponent<PlayerController>().CheckpointMeter = Instantiate(CheckpointMeterPrefab, FindObjectOfType<Canvas>().transform);
            currentPlayer.GetComponent<PlayerController>().CheckpointMeterFill = Instantiate(CheckpointMeterFillPrefab, FindObjectOfType<Canvas>().transform);*/
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
