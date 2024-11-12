using System.Collections.Generic;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.World.Tiles;
using UnityEngine;

namespace Minefactory.Player.Inventory
{
    public class Inventory : StorageBehaviour
    {
        public StorageData inventoryData;
        public ItemRegistry itemRegistry;
        public TileRegistry tileRegistry;

        public delegate void UseItem(ItemData item);
        public static UseItem useItem;

        private List<GameObject> cells = new List<GameObject>();



        // Start is called before the first frame update
        void Start()
        {
            // Get cell references
            for (int i = 0; i < transform.childCount; i++)
            {
                var cell = transform.GetChild(i);
                if (cell.CompareTag("InventoryCell"))
                {
                    var script = cell.GetComponent<InventorySlot>();
                    script.slotIndex = i;
                    cells.Add(cell.gameObject);
                }
            }
            useItem += RemoveItemFromInventory;
        }

        private bool inventoryInitialized = false;

        // Update is called once per frame
        void Update()
        {
            if (!inventoryInitialized)
            {
                inventoryInitialized = true;
                var item = itemRegistry.GetItem("belt");
                for (int i = 0; i < 20; i++)
                {
                    inventoryData.AddItem(item);
                }
                UpdateUI();
            }
            var toggleInventory = Input.GetKeyDown(KeyCode.I);
            if (toggleInventory)
            {
                var inventorySprite = GetComponent<SpriteRenderer>();
                inventorySprite.enabled = !inventorySprite.enabled;
                foreach (var cell in cells)
                {
                    cell.SetActive(!cell.activeSelf);
                }
            }
        }

        public void AddItem(ItemData item)
        {
            inventoryData.AddItem(item);
            UpdateUI();
        }

        public override void UpdateUI()
        {
            foreach (var cell in cells)
            {
                cell.GetComponent<InventorySlot>().UpdateItem();
            }
        }

        void RemoveItemFromInventory(ItemData item)
        {
            inventoryData.RemoveItem(item);
            UpdateUI();
        }

    }
}