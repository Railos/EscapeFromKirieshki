using UnityEngine;

public class PlayerStash : MonoBehaviour
{
    public GameObject stashObject;
    public GameObject cam;
    public KeyCode stashKey;

    private void Start()
    {
        stashObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(stashKey))
        {
            if (stashObject.activeSelf)
            {
                stashObject.SetActive(false);
                GetComponent<PlayerController>().enabled = true;
                cam.GetComponent<HeadBob>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                stashObject.SetActive(true);
                GetComponent<PlayerController>().enabled = false;
                cam.GetComponent<HeadBob>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
    
}
