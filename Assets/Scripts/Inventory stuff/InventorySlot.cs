using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    public bool isOccupied;
    public Vector2 index;
    
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (inventoryItem)
        {
            inventoryItem.isSuccesfull = true;
            inventoryItem.DoTheDrop(this);
        }
        
    }
}
