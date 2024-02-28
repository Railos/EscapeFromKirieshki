using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractionZone : MonoBehaviour
{
    public TMP_Text extractionTimer;
    public float timeToExit;
    public List<InventoryItem> itemsToExit;
    private float timer;
    
    public IEnumerator ExitingRaid()
    {
        extractionTimer.transform.parent.gameObject.SetActive(true);
        timer = timeToExit;
        while (timer != 0.0f)
        {
            timer -= 0.1f;
            if (timer < 0) timer = 0.0f;
            extractionTimer.text = timer.ToString("f1");
            yield return new WaitForSeconds(0.1f);
        }

        SceneManager.LoadScene("Main Menu");
    }

    public void HideTimer()
    {
        extractionTimer.transform.parent.gameObject.SetActive(false);
    }
}
