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
        if (!PlayerInventory.Instance.inStash && Input.GetKeyDown(pauseKey))
        {
            canvas.SetActive(!canvas.activeSelf);
            statsCanvas.SetActive(!statsCanvas.activeSelf);
            isPaused = !isPaused;
            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ReturnToGame()
    {
        statsCanvas.SetActive(true);
        canvas.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
