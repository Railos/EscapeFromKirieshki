using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;
    public GameObject canvas;
    public GameObject statsCanvas;
    public GameObject extractionCanvas;
    public KeyCode pauseKey;
    public bool isPaused;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (!PlayerInventory.Instance.inStash && Player.Instance.controls.Land.PauseGame.triggered)
        {
            canvas.SetActive(!canvas.activeSelf);
            statsCanvas.SetActive(!statsCanvas.activeSelf);
            isPaused = !isPaused;
            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GetComponent<InteractionHandler>().interactionMenu.gameObject.SetActive(false);
                extractionCanvas.SetActive(false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                GetComponent<InteractionHandler>().interactionMenu.gameObject.SetActive(true);
                if (Player.Instance.extractionCoroutine != null) extractionCanvas.SetActive(true);
            }
        }
        else if (PlayerInventory.Instance.inStash && Player.Instance.controls.Land.PauseGame.triggered) PlayerInventory.Instance.CloseTab();
    }

    public void GoToMainMenu()
    {
        Player.Instance.status = PlayerStatus.LeftTheGame;
        GameManager.Instance.UpdateGameState(GameState.FinishedRaid);
    }

    public void ReturnToGame()
    {
        statsCanvas.SetActive(true);
        canvas.SetActive(false);
        if (Player.Instance.extractionCoroutine != null) extractionCanvas.SetActive(true);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
