using System.Collections.Generic;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.World.Tiles;
using UnityEngine;

namespace Minefactory.Player.Inventory
{
    public class Inventory : MonoBehaviour
    {
        public StorageData inventory;
        public ItemRegistry itemRegistry;
        public TileRegistry tileRegistry;
        public Sprite cellSprite;
        public delegate void OnItemChange();
        public static OnItemChange onItemChange;
        public List<GameObject> cells;



        // Start is called before the first frame update
        void Start()
        {
            onItemChange += RefreshInventory;
            DisplayCells();
            var item = itemRegistry.GetItem("belt");
            inventory.AddItem(item);
            inventory.AddItem(item);
            inventory.AddItem(item);
            inventory.AddItem(item);
            inventory.AddItem(item);
            inventory.AddItem(item);
            inventory.AddItem(item);
            inventory.AddItem(item);
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
                    var script = itemObject.AddComponent<InventorySlot>();
                    script.stack = stack;
                    script.inventory = inventory;
                    script.tileRegistry = tileRegistry;
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
}