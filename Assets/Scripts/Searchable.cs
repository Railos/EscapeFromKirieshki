using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Searchable : Interactable
{
    public Vector2 containerSize;
    public InventoryManager containerManager;
    public GameObject containerCanvas;
    public List<InventoryItem> items = new List<InventoryItem>();
    public List<InventoryItem> possibleItems = new List<InventoryItem>();
    public GameObject searchPanel;
    public bool generated = false;

    public bool searchedContainer = false;
    public bool searchedAllItems = false;

    private int curX = 0;
    private int curY = 0;
    private int maxY = 1;

    [HideInInspector] public Coroutine SearchContainerCoroutine = null;
    [HideInInspector] public Coroutine SearchItemsCoroutine = null;

    public IEnumerator GenerateLoot()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var curItem in possibleItems)
        {
            if (curItem.itemSize.x > containerSize.x || curItem.itemSize.y > containerSize.y) continue;
            if (Random.Range(1, 100) <= curItem.rarityPercent)
            {
                if (curX + 1 + curItem.itemSize.x > containerSize.x)
                {
                    curY += maxY;
                    curX = 0;
                }
                int y = curY + (int)curItem.itemSize.y - 1;
                int x = curX;
                if (y > containerSize.y) continue;
                if (x > containerSize.x) continue;
                if ((int)curItem.itemSize.y > maxY) maxY = (int)curItem.itemSize.y;

                var cur = Instantiate(curItem, containerManager.itemsObject);
                cur.container = containerManager;
                cur.transform.position = new Vector3(containerManager.cells[y * (int) containerSize.x + x].transform.position.x + ((cur.itemSize.x - 1) * 50 / 2), containerManager.cells[y * (int) containerSize.x + x].transform.position.y + ((cur.itemSize.y - 1) * 50 / 2), 0f);
                cur.isSearched = false;
                cur.GetComponent<Image>().sprite = cur.SearchingSprite;
                items.Add(cur);
                bool unityidinaxyi = false;
                for (int y1 = y; y1 > y - curItem.itemSize.y; y--)
                {
                    for (int x1 = x; x1 < x + curItem.itemSize.x; x++)
                    {
                        containerManager.cells[y1 * (int) containerSize.x + x1].isOccupied = true;
                        cur.occupiedCells.Add(containerManager.cells[y1 * (int) containerSize.x + x1]);
                        if (cur.occupiedCells.Count == (int)cur.itemSize.x * (int)cur.itemSize.y)
                        {
                            unityidinaxyi = true;
                            break;
                        }
                        yield return null;
                    }

                    if (unityidinaxyi) break;
                    yield return null;
                }
                curX += (int) curItem.itemSize.x;
                yield return null;
            }
        }

        yield return null;

        generated = true;
    }

    public IEnumerator SearchContainer()
    {
        yield return new WaitForSeconds(2f);
        searchedContainer = true;
        searchPanel.SetActive(false);
        SearchItemsCoroutine = StartCoroutine(SearchItems());
    }

    public IEnumerator SearchItems()
    {
        foreach (var item in items)
        {
            if (item.isSearched) continue;
            yield return new WaitForSeconds(1f);
            item.isSearched = true;
            item.GetComponent<Image>().sprite = item.NormalSprite;
            yield return new WaitForSeconds(0.1f);
        }

        searchedAllItems = true;
    }

    public void StopSearchingContainer() => StopCoroutine(SearchContainerCoroutine);
    public void StopSearchingItems() => StopCoroutine(SearchItemsCoroutine);
}