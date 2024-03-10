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
        GameManager.Instance.state = GameState.Menu;
    }

    public void StartRaid(int raidTime)
    {
        GameManager.Instance.raidTimer = raidTime;
        GameManager.Instance.raidMap = "Raid";
        GameManager.Instance.UpdateGameState(GameState.Raid);
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
