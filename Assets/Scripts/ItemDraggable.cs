using System;
using UnityEngine;

public class ItemDraggable : MonoBehaviour
{
    private bool isDragging;
    [SerializeField] private ItemContainer container;
    public Vector2 itemSize;
    public void OnMouseDrag()
    {
        isDragging = true;
        transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + itemSize.x * .5f, Camera.main.ScreenToWorldPoint(Input.mousePosition).y + itemSize.y * 0.5f, -0.1f), 0.1f);
    }

    public void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x > container.startingCellPosition.x - 0.2f &&
            mousePos.x < container.startingCellPosition.x + (0.5f * container.sizeInCells.x) + 0.2f)
        {
            Vector2 endPos = new Vector3(Mathf.RoundToInt((mousePos.x - container.startingCellPosition.x) / container.cellSize.x), Mathf.Abs(Mathf.RoundToInt((mousePos.y - container.startingCellPosition.y) / container.cellSize.y)));
            Debug.Log(endPos);

            transform.position = new Vector3((container.startingCellPosition.x + endPos.x * 0.5f) + ((itemSize.x - 1) * 0.5f) / 2, (container.startingCellPosition.y - endPos.y * 0.5f) + ((itemSize.y - 1) * 0.5f) / 2, -0.1f);
        }
    }
}
