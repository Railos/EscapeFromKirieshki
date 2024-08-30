using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour
{
    RaycastHit hit;
    
    public float interactionRange;
    public Transform interactionMenu;
    public bool isMenuOpen;
    private List<Interaction> curInteractions = new List<Interaction>();
    private int curIndex = 0;
    public Color normalColor;
    public Color selecterColor;
    public GameObject lootTab;
    [HideInInspector] public Searchable curContainer;

    private void Start()
    {
        lootTab.SetActive(false);
    }

    private void Update()
    {
        if (GetComponent<PauseMenuManager>().isPaused) return;
        if (curInteractions.Count != 0)
        {
            ScrollMenu();
            Interact();
        }
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.collider.GetComponent<Interactable>() != null && !isMenuOpen)
            {
                isMenuOpen = true;
                foreach (var interaction in hit.transform.GetComponent<Interactable>().Interactions)
                {
                    var z = Instantiate(interaction.gameObject, interactionMenu);
                    curInteractions.Add(z.GetComponent<Interaction>());
                }
            }
        }
        else
        {
            isMenuOpen = false;
            while (interactionMenu.childCount > 0) 
            {
                DestroyImmediate(interactionMenu.GetChild(0).gameObject);
            }
            curIndex = 0;
            curInteractions.Clear();
        }
    }

    public void ScrollMenu()
    {
        if (Input.mouseScrollDelta.y != 0) curInteractions[curIndex].GetComponent<Image>().color = normalColor;
        if (Input.mouseScrollDelta.y < 0) curIndex++;
        if (Input.mouseScrollDelta.y > 0) curIndex--;
        if (curIndex == curInteractions.Count) curIndex = 0;
        if (curIndex < 0) curIndex = curInteractions.Count - 1;
        curInteractions[curIndex].gameObject.GetComponent<Image>().color = selecterColor;
    }

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            string methodName = curInteractions[curIndex].name;
            Invoke(methodName, 0f);
        }
    }
    
    public void Open()
    {
        Debug.Log("OPEN");
    }

    public void Close()
    {
        Debug.Log("CLOSE");
    }

    public void Search()
    {
        GetComponent<PlayerInventory>().OpenTab();
        lootTab.SetActive(true);
        Searchable container = hit.transform.GetComponent<Searchable>();
        curContainer = container;
        container.containerCanvas.SetActive(true);
        if (!container.generated) StartCoroutine(container.GenerateLoot());
        if (!container.searchedContainer)
            container.SearchContainerCoroutine = container.StartCoroutine(container.SearchContainer());
        else if (!container.searchedAllItems) container.SearchItemsCoroutine = container.StartCoroutine(container.SearchItems());
    }
}
