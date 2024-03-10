using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    
    public bool inRaid;
    public int raidTimer = 0;
    public string raidMap;
    public GameState state;

    private void Awake()
    {
        DontDestroyOnLoad (this);
		
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        if (newState == GameState.Menu)
        {
            inRaid = false;
            raidTimer = 0;
            raidMap = "";
            SceneManager.LoadScene("Main Menu");
        }

        if (newState == GameState.Raid)
        {
            inRaid = true;
            SceneManager.LoadScene(raidMap);
        }

        if (newState == GameState.FinishedRaid)
        {
            Debug.Log(Player.Instance.status);
            SceneManager.LoadScene("Main Menu");
        }
    }

}

public enum GameState
{
    Menu,
    Raid,
    FinishedRaid
}