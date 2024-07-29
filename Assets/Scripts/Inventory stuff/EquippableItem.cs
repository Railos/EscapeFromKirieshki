using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippableItem : InventoryItem
{
    public EquipableItemTypes equipType;
    private Vector2 prevSize;
    [Header("Optional")]
    [SerializeField] private GameObject slotsObject;

    private void Start()
    {
        prevSize = new Vector2(itemSize.x * 50f, itemSize.y * 50f);
        transform.SetAsLastSibling();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        GetComponent<RectTransform>().sizeDelta = new Vector2(prevSize.x,prevSize.y);

        if (equipType == EquipableItemTypes.Rig || equipType == EquipableItemTypes.Backpack)
        {
            slotsObject.SetActive(false);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;
        container.inventoryCanvas.GetComponent<Canvas>().sortingOrder = 1;
        transform.SetParent(container.itemsObject);
        if (!isSuccesfull)
        {
            if (occupiedCells[0].GetComponent<EquipSlot>())
            {
                prevSize = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
                GetComponent<RectTransform>().sizeDelta = new Vector2(occupiedCells[0].GetComponent<RectTransform>().sizeDelta.x,occupiedCells[0].GetComponent<RectTransform>().sizeDelta.y);
                transform.position = occupiedCells[0].transform.position;
                container = occupiedCells[0].container;
                slotsObject.SetActive(true);
            }
            else 
            {
                transform.position = prevPosition;
                foreach (var cell in occupiedCells) cell.isOccupied = true;
            }
        }
        else isSuccesfull = false;
    }

    public void DoTheDropEquippable(InventorySlot slot)
    {
        if (slot.isOccupied)
        {
            transform.position = prevPosition;
            foreach (var cell in occupiedCells) cell.isOccupied = true;
            return;
        }
        prevSize = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
        GetComponent<RectTransform>().sizeDelta = new Vector2(slot.GetComponent<RectTransform>().sizeDelta.x,slot.GetComponent<RectTransform>().sizeDelta.y);
        transform.position = slot.transform.position;
        container = slot.container;
        foreach (var cell in occupiedCells) cell.isOccupied = false;
        occupiedCells.Clear();
        occupiedCells.Add(slot);
        slot.isOccupied = true;
        
        if (slot.GetComponent<EquipSlot>() && (equipType == EquipableItemTypes.Rig || equipType == EquipableItemTypes.Backpack)) slotsObject.SetActive(true);
    }
    
    public override void DoTheDrop(InventorySlot slot)
    {
        InventoryManager prevContainer = container;
        if (slot.container != this.container)
        {
            container = slot.container;
            transform.SetParent(container.itemsObject);
        }
        
        if ((slot.index.y + 1) - itemSize.y < 0 || slot.index.x + itemSize.x > container.sizeInCells.x)
        {
            if (occupiedCells[0].GetComponent<EquipSlot>())
            {
                prevSize = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
                GetComponent<RectTransform>().sizeDelta = new Vector2(occupiedCells[0].GetComponent<RectTransform>().sizeDelta.x,occupiedCells[0].GetComponent<RectTransform>().sizeDelta.y);
                transform.position = occupiedCells[0].transform.position;
                container = occupiedCells[0].container;
                slotsObject.SetActive(true);
            }
            else 
            {
                transform.position = prevPosition;
                transform.SetParent(prevContainer.transform);
                container = prevContainer;
                foreach (var cell in occupiedCells) cell.isOccupied = true;
            }
            return;
        }
        for (int y = (int)slot.index.y; y > slot.index.y - itemSize.y; y--)
        {
            for (int x = (int)slot.index.x; x < slot.index.x + itemSize.x; x++)
            {
                if (container.cells[y * (int) container.sizeInCells.x + x].isOccupied)
                {
                    if (occupiedCells[0].GetComponent<EquipSlot>())
                    {
                        prevSize = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
                        GetComponent<RectTransform>().sizeDelta = new Vector2(occupiedCells[0].GetComponent<RectTransform>().sizeDelta.x,occupiedCells[0].GetComponent<RectTransform>().sizeDelta.y);
                        transform.position = occupiedCells[0].transform.position;
                        container = occupiedCells[0].container;
                        slotsObject.SetActive(true);
                    }
                    else 
                    {
                        transform.position = prevPosition;
                        transform.SetParent(prevContainer.transform);
                        container = prevContainer;
                        foreach (var cell in occupiedCells) cell.isOccupied = true;
                    }
                    return;
                }
            }
        }
        
        foreach (var cell in occupiedCells) cell.isOccupied = false;
        occupiedCells.Clear();

        for (int y = (int)slot.index.y; y > slot.index.y - itemSize.y; y--)
        {
            for (int x = (int)slot.index.x; x < slot.index.x + itemSize.x; x++)
            {
                container.cells[y * (int) container.sizeInCells.x + x].isOccupied = true;
                occupiedCells.Add(container.cells[y * (int) container.sizeInCells.x + x]);
            }
        }
        
        transform.position = new Vector3(slot.transform.position.x + ((itemSize.x - 1) * 50 / 2), slot.transform.position.y + ((itemSize.y - 1) * 50 / 2), 0f);
    }
}
