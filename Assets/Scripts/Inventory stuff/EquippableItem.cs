using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class EquippableItem : InventoryItem
{
    public EquipableItemTypes itemType;
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

        if (itemType == EquipableItemTypes.Rig || itemType == EquipableItemTypes.Backpack)
        {
            slotsObject.SetActive(false);
        }
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
        
        if (slot.GetComponent<EquipSlot>() && (itemType == EquipableItemTypes.Rig || itemType == EquipableItemTypes.Backpack)) slotsObject.SetActive(true);
    }
}
