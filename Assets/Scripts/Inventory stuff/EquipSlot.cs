using UnityEngine;
using UnityEngine.EventSystems;

public class EquipSlot : InventorySlot
{
    public EquipableItemTypes equipType;
    
    public override void OnDrop(PointerEventData eventData)
    {
        EquippableItem inventoryItem = eventData.pointerDrag.GetComponent<EquippableItem>();
        if (inventoryItem is null) return;
        inventoryItem.container.inventoryCanvas.GetComponent<Canvas>().sortingOrder = 1;
        if (inventoryItem != null)
        {
            if (equipType == inventoryItem.equipType)
            {
                inventoryItem.isSuccesfull = true;
                inventoryItem.DoTheDropEquippable(this);
            }
        }
        
    }
}

public enum EquipableItemTypes
{
    BodyArmor,
    Rig,
    Backpack,
    Helmet,
    Headset
}