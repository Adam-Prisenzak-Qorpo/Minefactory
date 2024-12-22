using System.Collections.Generic;
using Minefactory.Game;
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
            var item = itemRegistry.GetItem("belt");
            inventoryData.AddItems(item, 20);
            var furnace = itemRegistry.GetItem("furnace");
            inventoryData.AddItem(furnace);
            var rawIron = itemRegistry.GetItem("iron_raw");
            var minerOutput = itemRegistry.GetItem("void_storage");
            var autoMiner = itemRegistry.GetItem("auto_miner");
            for (int i = 0; i < 20; i++)
            {
                inventoryData.AddItem(rawIron);
                inventoryData.AddItem(minerOutput);
                inventoryData.AddItem(autoMiner);
            }

            inventoryData.AddItems(rawIron, 6);
            var crafter = itemRegistry.GetItem("crafter");
            inventoryData.AddItem(crafter);

            var rawGold = itemRegistry.GetItem("gold_raw");
            inventoryData.AddItems(rawGold, 6);

            var gold = itemRegistry.GetItem("gold");
            inventoryData.AddItem(gold);

            var colony = itemRegistry.GetItem("colony");
            inventoryData.AddItem(colony);

            UpdateUI();
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