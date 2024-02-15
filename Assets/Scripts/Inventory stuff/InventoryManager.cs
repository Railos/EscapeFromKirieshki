using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventorySlot> cells;
    public Vector2 sizeInCells;
    public GameObject cell;
    public Transform cellsObject;
    public Transform itemsObject;

    private void Awake()
    {
        for (int i = 0; i < sizeInCells.x * sizeInCells.y; i++)
        {
            var curCell = Instantiate(cell, cellsObject);
            curCell.GetComponent<InventorySlot>().index = new Vector2(i % sizeInCells.x, i / (int)sizeInCells.x);
            cells.Add(curCell.GetComponent<InventorySlot>());
        }
        itemsObject.SetAsLastSibling();
    }
}
