using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Vector2 sizeInCells;
    public Vector2 cellSize;
    public Transform cellsObject;
    public Vector2 startingCellPosition;
    public GameObject cell;
    public List<ItemCell> cells;

    private void Start()
    {
        GenerateCells();
    }

    private void GenerateCells()
    {
        for (int i = 0; i < sizeInCells.x * sizeInCells.y; i++)
        {
            var curCell = Instantiate(cell, new Vector3(startingCellPosition.x + cellSize.x * (i % sizeInCells.x), startingCellPosition.y - cellSize.y * (i / (int)sizeInCells.x)), Quaternion.identity, cellsObject);
            cells.Add(curCell.GetComponent<ItemCell>());
        }
    }
}
