using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public PlayerStatus status;

    [Header("Stamina")]
    public Slider staminaBar;
    public GameObject staminaBarFill;
    public Color normalColorStamina;
    public Color exhaustedColorStamina;
    [SerializeField] private float stamina;
    [SerializeField] private float health;
    public float maxStamina = 5f;
    public float minStaminaForAction = 0.5f;
    public bool isDrainingStamina;

    public Coroutine extractionCoroutine;
    public Controls controls;
    
    public float Stamina
    {
        get
        {
            return stamina;
        }
        set
        {
            stamina = value;
            if (stamina < 0f)
            {
                stamina = 0f;
                GetComponent<PlayerController>().isRunning = false;
            }

            staminaBar.value = stamina;
            if (stamina < minStaminaForAction)
                staminaBarFill.GetComponent<Image>().color = exhaustedColorStamina;
            else staminaBarFill.GetComponent<Image>().color = normalColorStamina;
            isDrainingStamina = true;
            StopCoroutine(nameof(RestoreStamina));
            StartCoroutine(nameof(RestoreStamina));
        }
    }

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            if (health < 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        health = 0;
        status = PlayerStatus.KIA;
        GameManager.Instance.UpdateGameState(GameState.FinishedRaid);
    }

    private IEnumerator RestoreStamina()
    {
        yield return new WaitForSeconds(2f);
        isDrainingStamina = false;
        while (!isDrainingStamina && stamina < maxStamina)
        {
            stamina += 0.1f;
            if (stamina > maxStamina) stamina = maxStamina;
            staminaBar.value = stamina;
            if (stamina >= minStaminaForAction) staminaBarFill.GetComponent<Image>().color = normalColorStamina;
            yield return new WaitForSeconds(0.2f);
        }
    }

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

        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
        controls = new Controls();
        controls.Enable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ExtractionZone>() != null)
        {
            extractionCoroutine = StartCoroutine(other.GetComponent<ExtractionZone>().ExitingRaid());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ExtractionZone>() != null)
        {
            StopCoroutine(extractionCoroutine);
            other.GetComponent<ExtractionZone>().HideTimer();
            extractionCoroutine = null;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<Enemy>()) Health -= 10f;
    }
}

public enum PlayerStatus
{
    Alive,
    KIA,
    Lost,
    LeftTheGame,
    Survived
}