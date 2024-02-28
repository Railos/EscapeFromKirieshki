using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public bool inStash;
    public GameObject stashObject;
    public GameObject cam;
    public KeyCode stashKey;

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
    
    private void Start()
    {
        stashObject.SetActive(false);
    }

    private void Update()
    {
        if (!PauseMenuManager.Instance.isPaused && Player.Instance.controls.Land.OpenInventory.triggered)
        {
            if (stashObject.activeSelf)
            {
                inStash = false;
                stashObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                GetComponent<PlayerController>().Enabled = true;
            }
            else
            {
                inStash = true;
                stashObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GetComponent<PlayerController>().Enabled = false;
            }
        }
    }
    
}
