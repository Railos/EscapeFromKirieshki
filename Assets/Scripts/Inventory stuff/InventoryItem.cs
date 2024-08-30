using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryManager container;
    public List<InventorySlot> occupiedCells = new List<InventorySlot>();
    public Vector2 itemSize;
    public bool isSuccesfull = false;
    public Vector2 prevPosition;
    public ItemTypes type;
    public float rarityPercent;
    public bool isSearched = true;
    public Sprite SearchingSprite;
    public Sprite NormalSprite;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isSearched) return;
        GetComponent<Image>().raycastTarget = false;
        prevPosition = transform.position;
        transform.SetParent(container.inventoryCanvas.transform);
        container.inventoryCanvas.GetComponent<Canvas>().sortingOrder = 2;
        transform.SetAsLastSibling();
        foreach (var cell in occupiedCells) cell.isOccupied = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (isSearched) transform.position = new Vector3(Input.mousePosition.x + ((itemSize.x - 1) * 50 / 2), Input.mousePosition.y + ((itemSize.y - 1) * 50 / 2), 0f);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!isSearched) return;
        GetComponent<Image>().raycastTarget = true;
        container.inventoryCanvas.GetComponent<Canvas>().sortingOrder = 1;
        transform.SetParent(container.itemsObject);
        if (!isSuccesfull)
        {
            transform.position = prevPosition;
            foreach (var cell in occupiedCells) cell.isOccupied = true;
        }
        else isSuccesfull = false;
    }

    public virtual void DoTheDrop(InventorySlot slot)
    {
        if (!isSearched) return;
        InventoryManager prevContainer = container;
        if (slot.container != this.container)
        {
            container = slot.container;
            transform.SetParent(container.itemsObject);
        }
        
        if ((slot.index.y + 1) - itemSize.y < 0 || slot.index.x + itemSize.x > container.sizeInCells.x)
        {
            transform.position = prevPosition;
            foreach (var cell in occupiedCells) cell.isOccupied = true;
            container = prevContainer;
            return;
        }
        for (int y = (int)slot.index.y; y > slot.index.y - itemSize.y; y--)
        {
            for (int x = (int)slot.index.x; x < slot.index.x + itemSize.x; x++)
            {
                if (container.cells[y * (int) container.sizeInCells.x + x].isOccupied)
                {
                    transform.position = prevPosition;
                    foreach (var cell in occupiedCells) cell.isOccupied = true;
                    container = prevContainer;
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

public enum ItemTypes
{
    BuildingMaterials,
    Valuables,
    Electronics,
    Keys,
    Equippable
}