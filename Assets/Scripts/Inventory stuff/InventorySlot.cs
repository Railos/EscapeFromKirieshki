using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventoryManager container;
    public bool isOccupied;
    public Vector2 index;
    
    public virtual void OnDrop(PointerEventData eventData)
    {
        InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (inventoryItem is null) return;
        inventoryItem.container.inventoryCanvas.GetComponent<Canvas>().sortingOrder = 1;
        if (inventoryItem != null)
        {
            inventoryItem.isSuccesfull = true;
            inventoryItem.DoTheDrop(this);
        }
        
    }
}
