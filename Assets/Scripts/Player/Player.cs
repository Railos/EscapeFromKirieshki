using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Stamina")]
    public Slider staminaBar;
    public GameObject staminaBarFill;
    public Color normalColorStamina;
    public Color exhaustedColorStamina;
    [SerializeField] private float stamina;
    public float maxStamina = 5f;
    public float minStaminaForAction = 0.5f;
    public bool isDrainingStamina;
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
    }
}
