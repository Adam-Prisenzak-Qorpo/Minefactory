using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventoryClass inventory;
    public Sprite cellSprite;
    public delegate void OnItemAdded();
    public static OnItemAdded onItemAdded;
    public List<GameObject> cells;

    // Start is called before the first frame update
    void Start()
    {
        onItemAdded += RefreshInventory;
        DisplayCells();
    }

    // Update is called once per frame
    void Update()
    {
        var toggleInventory = Input.GetKeyDown(KeyCode.I);
        if (toggleInventory)
        {
            var inventorySprite = GetComponent<SpriteRenderer>();
            inventorySprite.enabled = !inventorySprite.enabled;
            if (inventorySprite.enabled)
            {
                DisplayCells();
            }
            else
            {
                ClearCells();
            }
        }
    }

    void ClearCells()
    {
        foreach (var cell in cells)
        {
            Destroy(cell);
        }
        cells.Clear();
    }

    // 2 x 10 grid. Cell size is 16x16
    void DisplayCells()
    {
        for (int i = 0; i < 20; i++)
        {
            var x = i % 10;
            var y = i / 10;
            var cell = new GameObject("Cell-" + i);
            cell.transform.parent = transform;
            cell.transform.localPosition = new Vector2(x - 4.5f, y - .5f);
            cell.transform.localScale = Vector2.one;
            var cellRenderer = cell.AddComponent<SpriteRenderer>();
            cellRenderer.sprite = cellSprite;
            cellRenderer.sortingOrder = 1;

            var stack = inventory.GetItemStack(i);
            if (stack != null)
            {
                var itemObject = new GameObject("Item");
                itemObject.transform.parent = cell.transform;
                itemObject.transform.localScale = Vector2.one;
                itemObject.transform.localPosition = Vector2.zero;

                var itemRenderer = itemObject.AddComponent<SpriteRenderer>();
                itemRenderer.sprite = stack.item.sprite;
                itemRenderer.sortingOrder = 2;

                // Add text
                var textObject = new GameObject("Text");
                textObject.transform.parent = cell.transform;
                textObject.transform.localScale = new Vector2(0.1f, 0.1f);
                textObject.transform.localPosition = new Vector2(-0.25f, 0.25f);

                var text = textObject.AddComponent<TextMesh>();
                text.text = stack.amount.ToString();
                text.fontSize = 100;
                text.characterSize = 0.3f;
                text.anchor = TextAnchor.MiddleCenter;
                text.color = Color.white;

                var textRenderer = textObject.GetComponent<Renderer>();
                textRenderer.sortingOrder = 3;
            }
            cells.Add(cell);
        }
    }

    void RefreshInventory()
    {
        if (cells.Count > 0)
        {
            ClearCells();
            DisplayCells();
        }
    }
}
