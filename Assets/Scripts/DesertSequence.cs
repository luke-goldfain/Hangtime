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
    private GameObject MainCanvas;
    void Start()
    {
        StartCoroutine (TheSequence());
    }
    IEnumerator TheSequence () // Plays each section of the animation one at a time.
    {
        yield return new WaitForSeconds(4);
        Cam2.SetActive(true);
        Cam1.SetActive(false);

        yield return new WaitForSeconds(6);
        Cam3.SetActive(true);
        Cam2.SetActive(false);

        yield return new WaitForSeconds(5);
        Cam4.SetActive(true);
        Cam3.SetActive(false);

        

        yield return new WaitForSeconds(8);
        Cam5.SetActive(true);
        FollowOrb.SetActive(true);
        MainCanvas.SetActive(true);
        Cam4.SetActive(false);

       

        yield return new WaitForSeconds(27);
       SpawnManager.SetActive(true);
        FollowOrb.SetActive(true);
        MainCanvas.SetActive(true);
        Cam5.SetActive(false);
    }

}
