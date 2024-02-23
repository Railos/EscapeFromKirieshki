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
        if (!PauseMenuManager.Instance.isPaused && Input.GetKeyDown(stashKey))
        {
            if (stashObject.activeSelf)
            {
                inStash = false;
                stashObject.SetActive(false);
                GetComponent<PlayerController>().enabled = true;
                cam.GetComponent<HeadBob>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                inStash = true;
                stashObject.SetActive(true);
                GetComponent<PlayerController>().enabled = false;
                cam.GetComponent<HeadBob>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
    
}
