using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    public GameObject PlayerPrefab, HUDPrefab, PauseMenuPrefab,
                      PinkyPrefab, SongbirdPrefab;
    //public GameObject PlayerPrefab, ReticlePrefab, SpeedometerPrefab, CheckpointMeterPrefab, CheckpointMeterFillPrefab;

    [SerializeField, Tooltip("The initial spawn points of each player in the current scene.")]
    public List<Vector3> InitialSpawnPoints = new List<Vector3>();

    [SerializeField, Tooltip("(Unfunctional) The Y rotation in degrees of the spawn point.")]
    public float SpawnRotation = 0f;

    // The number of players that have been spawned before the currently iterated player.
    // Used to determine which camera view to assign to the player.
    private int numOfPlayersSpawned;

    // Start is called before the first frame update
    void Start()
    {
        numOfPlayersSpawned = 0;

        if (InitialSpawnPoints.Count < 4)
        {
            InitialSpawnPoints.Add(new Vector3(1 * InitialSpawnPoints.Count, 2, 0));
        }

        for (int i = 1; i <= GameStats.PlayersReady.Length; i++)
        {
            if (GameStats.PlayersReady[i - 1] == true)
            {
                GameObject currentPlayer = new GameObject();

                switch (GameStats.chosenChars[i - 1])
                {
                    case GameStats.charChoices.pinky:
                        currentPlayer = Instantiate(PinkyPrefab, InitialSpawnPoints[i - 1], Quaternion.identity);
                        break;
                    case GameStats.charChoices.songbird:
                        currentPlayer = Instantiate(SongbirdPrefab, InitialSpawnPoints[i - 1], Quaternion.identity);
                        break;
                }
                
                currentPlayer.transform.Rotate(0f, SpawnRotation, 0f); // DOESN'T WORK >:(
                currentPlayer.GetComponent<PlayerController>().PlayerNumber = i;

                currentPlayer.GetComponent<PlayerController>().HUD = Instantiate(HUDPrefab, Vector3.zero, Quaternion.identity);

                // TODO: Pause menu not currently functional. View PauseManager.cs for more detail
                //currentPlayer.GetComponent<PauseManager>().PauseMenu = Instantiate(PauseMenuPrefab, Vector3.zero, Quaternion.identity);
                //currentPlayer.GetComponent<PauseManager>().PauseMenu.SetActive(false);


                numOfPlayersSpawned++;

                currentPlayer.GetComponent<PlayerController>().PlayerViewNumber = numOfPlayersSpawned; // Assign camera number
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
