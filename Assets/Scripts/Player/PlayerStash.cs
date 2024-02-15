using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStash : MonoBehaviour
{
    public GameObject stashObject;
    public KeyCode stashKey;

    private void Start()
    {
        stashObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(stashKey))
        {
            if (stashObject.activeSelf)
            {
                stashObject.SetActive(false);
                GetComponent<PlayerLook>().enabled = true;
                GetComponent<PlayerMove>().enabled = true;
            }
            else
            {
                stashObject.SetActive(true);
                GetComponent<PlayerLook>().enabled = false;
                GetComponent<PlayerMove>().enabled = false;
            }
        }
    }
    
}
