using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public bool generateOnAwake;
    public List<InventorySlot> cells;
    public Vector2 sizeInCells;
    public GameObject cell;
    public Transform cellsObject;
    public Transform itemsObject;
    public GameObject inventoryCanvas;

    private void Awake()
    {
        if (generateOnAwake) GenerateCells();
    }

    public void GenerateCells()
    {
        for (int i = 0; i < sizeInCells.x * sizeInCells.y; i++)
        {
            var curCell = Instantiate(cell, cellsObject);
            curCell.GetComponent<InventorySlot>().index = new Vector2(i % sizeInCells.x, i / (int)sizeInCells.x);
            curCell.GetComponent<InventorySlot>().container = this;
            cells.Add(curCell.GetComponent<InventorySlot>());
        }
        itemsObject.SetAsLastSibling();
    }

    public void ClearCells()
    {
        foreach (var cell in cells)
        {
            DestroyImmediate(cell.gameObject);
        }
        cells.Clear();
    }
}
