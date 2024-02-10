using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDraggable : MonoBehaviour
{
    private bool isDragging;
    [SerializeField] private ItemContainer container;
    public List<ItemCell> occupiedCells = new List<ItemCell>();
    public Vector2 itemSize;
    private Vector3 prevPos;

    public void OnMouseDown()
    {
        foreach (var cell in occupiedCells) cell.isOccupied = false;
    }

    public void OnMouseDrag()
    {
        isDragging = true;
        Vector3 mousePos = GetWorldPositionOnPlane(Input.mousePosition, 0f);
        transform.position = Vector3.Lerp(transform.position, new Vector3(mousePos.x + ((itemSize.x - 1) * 0.5f / 2), mousePos.y + ((itemSize.y - 1) * 0.5f / 2), 0f), 0.1f);
    }

    public void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        Vector3 mousePos = GetWorldPositionOnPlane(Input.mousePosition, 0f);
        if (mousePos.x > container.startingCellPosition.x - 0.2f &&
            mousePos.x < container.startingCellPosition.x + (0.5f * container.sizeInCells.x) + 0.2f && mousePos.y < container.startingCellPosition.y + 0.2f && mousePos.y > container.startingCellPosition.y - (0.5f * container.sizeInCells.y))
        {
            Vector2 endPos = new Vector3(Mathf.RoundToInt((mousePos.x - container.startingCellPosition.x) / container.cellSize.x), Mathf.Abs(Mathf.RoundToInt((mousePos.y - container.startingCellPosition.y) / container.cellSize.y)));
            Debug.Log(endPos);
            for (int y = (int)endPos.y; y > endPos.y - itemSize.y; y--)
            {
                for (int x = (int) endPos.x; x < endPos.x + itemSize.x; x++)
                {
                    if (container.cells[y * (int) container.sizeInCells.x + x].isOccupied)
                    {
                        transform.position = prevPos;
                        foreach (var cell in occupiedCells) cell.isOccupied = true;
                        return;
                    }
                }
            }

            foreach (var cell in occupiedCells) cell.isOccupied = false;
            occupiedCells.Clear();
            
            for (int y = (int)endPos.y; y > endPos.y - itemSize.y; y--)
            {
                for (int x = (int) endPos.x; x < endPos.x + itemSize.x; x++)
                {
                    container.cells[y * (int) container.sizeInCells.x + x].isOccupied = true;
                    occupiedCells.Add(container.cells[y * (int) container.sizeInCells.x + x]);
                }
            }
            
            transform.position = new Vector3((container.startingCellPosition.x + endPos.x * 0.5f) + ((itemSize.x - 1) * 0.5f) / 2, (container.startingCellPosition.y - endPos.y * 0.5f) + ((itemSize.y - 1) * 0.5f) / 2, -0.1f);
            prevPos = transform.position;
        }
        else
        {
            transform.position = prevPos;
            foreach (var cell in occupiedCells) cell.isOccupied = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && isDragging)
        {
            float z = itemSize.x;
            itemSize.x = itemSize.y;
            itemSize.y = z;
            
            transform.Rotate(0, 0, -90);
        }
    }
    
    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}
