using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Start Desert animation before the game begins.
/// Music and count down should begin after animation is complete.
/// Gobby and K1 conversation should play during this opening animation.
/// 48 second animation sequence.
/// </summary>


public class DesertSequence : MonoBehaviour
{
    [SerializeField]
    private GameObject SkipText;
    [SerializeField]
    private GameObject Cam1;
    [SerializeField]
    private GameObject Cam2;
    [SerializeField]
    private GameObject Cam3;
    [SerializeField]
    private GameObject Cam4;
    [SerializeField]
    private GameObject Cam5;
    [SerializeField]
    private GameObject FollowOrb;
    [SerializeField]
    private GameObject SpawnManager;
    [SerializeField]
    private GameObject CountdownText;

    private bool cutsceneIsPlaying;

    private Coroutine sequence;

    void Start()
    {
        sequence = StartCoroutine (TheSequence());
    }


    private void Update()
    {
        if (cutsceneIsPlaying && 
            (Input.GetButtonDown("P1Start") || 
             Input.GetButtonDown("P2Start") ||
             Input.GetButtonDown("P3Start") ||
             Input.GetButtonDown("P4Start")))
        {
            StopCoroutine(sequence);

            cutsceneIsPlaying = false;

            SkipText.SetActive(false);

            // Set any active cameras to inactive, with the exception of Cam1, because it contains an audio listener.
            if (Cam1.GetComponent<Camera>().enabled) Cam1.GetComponent<Camera>().enabled = false;
            if (Cam2.activeInHierarchy) Cam2.SetActive(false);
            if (Cam3.activeInHierarchy) Cam3.SetActive(false);
            if (Cam4.activeInHierarchy) Cam4.SetActive(false);
            if (Cam5.activeInHierarchy) Cam5.SetActive(false);

            // Set gameplay-relevant objects active.
            FollowOrb.SetActive(true);
            CountdownText.SetActive(true);
            SpawnManager.SetActive(true);
        }
    }

    IEnumerator TheSequence () // Plays each section of the animation one at a time.
    {
        cutsceneIsPlaying = true;

        yield return new WaitForSeconds(4);
        Cam2.SetActive(true);
        Cam1.GetComponent<Camera>().enabled = false;

        yield return new WaitForSeconds(6);
        Cam3.SetActive(true);
        Cam2.SetActive(false);

        yield return new WaitForSeconds(5);
        Cam4.SetActive(true);
        Cam3.SetActive(false);

        

        yield return new WaitForSeconds(8);
        Cam5.SetActive(true);
        Cam4.SetActive(false);


        yield return new WaitForSeconds(27);
        FollowOrb.SetActive(true);
        CountdownText.SetActive(true);
        SpawnManager.SetActive(true);
        Cam5.SetActive(false);
        SkipText.SetActive(false);

        cutsceneIsPlaying = false;
    }

}
