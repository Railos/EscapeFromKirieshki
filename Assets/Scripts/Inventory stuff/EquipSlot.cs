using UnityEngine;
using UnityEngine.EventSystems;

public class EquipSlot : InventorySlot
{
    public EquipableItemTypes itemType;
    
    public override void OnDrop(PointerEventData eventData)
    {
        EquippableItem inventoryItem = eventData.pointerDrag.GetComponent<EquippableItem>();
        if (inventoryItem != null)
        {
            if (itemType == inventoryItem.itemType)
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