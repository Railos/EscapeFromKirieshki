using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private InventoryManager container;
    public List<InventorySlot> occupiedCells = new List<InventorySlot>();
    public Vector2 itemSize;
    public bool isSuccesfull = false;
    public Vector2 prevPosition;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = false;
        prevPosition = transform.position;
        transform.SetParent(container.transform);
        foreach (var cell in occupiedCells) cell.isOccupied = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(Input.mousePosition.x + ((itemSize.x - 1) * 50 / 2), Input.mousePosition.y + ((itemSize.y - 1) * 50 / 2), 0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;
        transform.SetParent(container.itemsObject);
        if (!isSuccesfull)
        {
            transform.position = prevPosition;
            foreach (var cell in occupiedCells) cell.isOccupied = true;
        }
        else isSuccesfull = false;
    }

    public void DoTheDrop(InventorySlot slot)
    {
        if ((slot.index.y + 1) - itemSize.y < 0 || slot.index.x + itemSize.x > container.sizeInCells.x)
        {
            transform.position = prevPosition;
            foreach (var cell in occupiedCells) cell.isOccupied = true;
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
