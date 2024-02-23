using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public GameObject stashCanvas;
    public GameObject mainCanvas;

    private void Start()
    {
        GoBackToMain();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartRaid()
    {
        SceneManager.LoadScene("Raid");
    }

    public void OpenStash()
    {
        mainCanvas.SetActive(false);
        stashCanvas.SetActive(true);
    }

    public void GoBackToMain()
    {
        mainCanvas.SetActive(true);
        stashCanvas.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
