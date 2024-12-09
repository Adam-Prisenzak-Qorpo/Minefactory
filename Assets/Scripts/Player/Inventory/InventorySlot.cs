using System;
using Minefactory.Common;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.World;
using Minefactory.World.Tiles;
using Unity.VisualScripting;
using UnityEngine;

namespace Minefactory.Player.Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        public StorageData inventory;
        public int slotIndex;
        public GameObject textContainer;
        public GameObject itemContainer;
        
        private SpriteRenderer itemRenderer;
        private InventoryItem itemScript;
        private TextMesh text;

        public void Awake()
        {
            itemRenderer = itemContainer.GetComponent<SpriteRenderer>();
            itemScript = itemContainer.GetComponent<InventoryItem>();
            text = textContainer.GetComponent<TextMesh>();
        }

        public void UpdateItem()
        {

            itemScript.itemStack = inventory.GetItemStack(slotIndex);
            if (itemScript.itemStack is null)
            {
                itemRenderer.sprite = null;
                text.text = "";
                return;
            }

            itemRenderer.sprite = itemScript.itemStack.item.sprite;
            text.text = itemScript.itemStack.amount.ToString();
        }
    }
}