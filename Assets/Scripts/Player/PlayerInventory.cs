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
                CloseTab();
            }
            else
            {
                OpenTab();
            }
        }
    }

    public void OpenTab()
    {
        inStash = true;
        stashObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GetComponent<PlayerController>().Enabled = false;
        GetComponent<InteractionHandler>().interactionMenu.gameObject.SetActive(false);
        
    }

    public void CloseTab()
    {
        inStash = false;
        stashObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GetComponent<PlayerController>().Enabled = true;
        InteractionHandler interactionHandler = GetComponent<InteractionHandler>();
        interactionHandler.interactionMenu.gameObject.SetActive(true);
        interactionHandler.lootTab.SetActive(false);
        if (interactionHandler.curContainer != null) interactionHandler.curContainer.containerCanvas.SetActive(false);
    }
    
}
