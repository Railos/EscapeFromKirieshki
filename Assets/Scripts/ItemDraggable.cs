using System;
using UnityEngine;

public class ItemDraggable : MonoBehaviour
{
    private bool isDragging;
    [SerializeField] private ItemContainer container;
    public Vector2 itemSize;
    private Vector2 prevPos;
    private Vector2 prevEndPos;

    public void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 endPos = new Vector3(Mathf.RoundToInt((mousePos.x - container.startingCellPosition.x) / container.cellSize.x), Mathf.Abs(Mathf.RoundToInt((mousePos.y - container.startingCellPosition.y) / container.cellSize.y)));
        prevPos = new Vector3((container.startingCellPosition.x + endPos.x * 0.5f) + ((itemSize.x - 1) * 0.5f) / 2, (container.startingCellPosition.y - endPos.y * 0.5f) + ((itemSize.y - 1) * 0.5f) / 2, -0.1f);
        prevEndPos = endPos;
    }

    public void OnMouseDrag()
    {
        isDragging = true;
        transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + ((itemSize.x - 1) * 0.5f / 2), Camera.main.ScreenToWorldPoint(Input.mousePosition).y + ((itemSize.y - 1) * 0.5f / 2), -0.1f), 0.1f);
    }

    public void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x > container.startingCellPosition.x - 0.2f &&
            mousePos.x < container.startingCellPosition.x + (0.5f * container.sizeInCells.x) + 0.2f && mousePos.y < container.startingCellPosition.y + 0.2f && mousePos.y > container.startingCellPosition.y - (0.5f * container.sizeInCells.y))
        {
            Vector2 endPos = new Vector3(Mathf.RoundToInt((mousePos.x - container.startingCellPosition.x) / container.cellSize.x), Mathf.Abs(Mathf.RoundToInt((mousePos.y - container.startingCellPosition.y) / container.cellSize.y)));
            Debug.Log(endPos);
            for (int y = (int) endPos.y; y < (int) endPos.y + itemSize.y; y++)
            {
                for (int x = (int) endPos.x; x < (int) endPos.x + itemSize.x; x++)
                {
                    if (container.cells[(int)container.sizeInCells.x * y + x].isOccupied)
                    {
                        transform.position = prevPos;
                        return;
                    }
                }
            }

            transform.position = new Vector3((container.startingCellPosition.x + endPos.x * 0.5f) + ((itemSize.x - 1) * 0.5f) / 2, (container.startingCellPosition.y - endPos.y * 0.5f) + ((itemSize.y - 1) * 0.5f) / 2, -0.1f);
            for (int y = (int) prevEndPos.y; y < (int) prevEndPos.y + itemSize.y; y++)
            {
                for (int x = (int) prevEndPos.x; x < (int) prevEndPos.x + itemSize.x; x++)
                {
                    container.cells[(int)container.sizeInCells.x * y + x].isOccupied = false;
                }
            }
            
            for (int y = (int) endPos.y; y < (int) endPos.y + itemSize.y; y++)
            {
                for (int x = (int) endPos.x; x < (int) endPos.x + itemSize.x; x++)
                {
                    container.cells[(int)container.sizeInCells.x * y + x].isOccupied = true;
                }
            }
        }
        else
        {
            transform.position = prevPos;
        }
    }
}
