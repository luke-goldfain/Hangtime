using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectEnabler : MonoBehaviour
{
    [SerializeField]
    bool closeThisObjectWithCancelButton;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (closeThisObjectWithCancelButton)
        {
            if (Input.GetButtonDown("P1Cancel") ||
                Input.GetButtonDown("P2Cancel") ||
                Input.GetButtonDown("P3Cancel") ||
                Input.GetButtonDown("P4Cancel"))
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    public void EnableGameObject(GameObject go)
    {
        go.SetActive(true);
    }

    public void DisableGameObject(GameObject go)
    {
        go.SetActive(false);
    }
}
