using System.Collections;
using TMPro;
using UnityEngine;

public class RaidTimer : MonoBehaviour
{

    private float timer;
    [SerializeField] private TMP_Text timerText;

    private void Start()
    {
        Player.Instance.status = PlayerStatus.Alive;
        timer = GameManager.Instance.raidTimer;
        timerText.text = ((int)(timer / 3600 % 24)).ToString() + ':' + ((int)(timer % 3600 / 60 / 10)).ToString() + ((int)(timer % 3600 / 60 % 10)).ToString() + ':' +
                         ((int)(timer % 60 / 10)).ToString() + ((int)(timer % 60 % 10)).ToString();
        StartCoroutine(nameof(DecreasingTimer));
    }
    
    private IEnumerator DecreasingTimer()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer -= 1;
            timerText.text = ((int)(timer / 3600 % 24)).ToString() + ':' + ((int)(timer % 3600 / 60 / 10)).ToString() + ((int)(timer % 3600 / 60 % 10)).ToString() + ':' +
                             ((int)(timer % 60 / 10)).ToString() + ((int)(timer % 60 % 10)).ToString();
        }

        Player.Instance.status = PlayerStatus.Lost;
        GameManager.Instance.UpdateGameState(GameState.FinishedRaid);
    }
}
