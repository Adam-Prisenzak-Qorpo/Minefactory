using System;
using Minefactory.Common;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.Common.Behaviour;
using Minefactory.Factories.Recipes;
using UnityEngine;
using Minefactory.Factories;
using Minefactory.Game;
namespace Minefactory.World.Tiles.Behaviour
{
    public class FurnaceTileBehaviour : BreakableTileBehaviour
    {
        public ItemRegistry itemRegistry;

        private StorageData storage;
        private Collision2DProxy inputCollider;
        private Collision2DProxy outputCollider;
        private ItemRecipe recipe;
        private GameObject furnaceUI;

        void Start()
        {
            furnaceUI = WorldManager.Instance.GetUIManager().furnaceUI;
            storage = ScriptableObject.CreateInstance<StorageData>();

            inputCollider = transform.Find("Input").GetComponent<Collision2DProxy>();
            if (inputCollider is null)
            {
                Debug.LogError("Input collider is null");
                return;
            }
            inputCollider.OnTriggerStay2D_Action += Input_OnTriggerStay2D;

            outputCollider = transform.Find("Output").GetComponent<Collision2DProxy>();
            if (outputCollider is not null) return;
            Debug.LogError("Output collider is null");
        }

        private bool isHovered = false;

        private void OnMouseEnter()
        {
            isHovered = true;
        }

        private void OnMouseExit()
        {
            isHovered = false;
        }



        void Update()
        {
            var mouseDown = Input.GetMouseButtonDown(1);
            if (mouseDown && isHovered && !furnaceUI.activeSelf)
            {
                var script = furnaceUI.GetComponent<FurnaceBehaviour>();
                script.SelectRecipe(recipe);
                script.selectRecipeEvent.AddListener(OnSelectRecipe);
                WorldManager.Instance.GetUIManager().OpenFurnaceUI();
            }

        }

        private void OnSelectRecipe(ItemRecipe newRecipe)
        {
            recipe = newRecipe;
        }

        private void Input_OnTriggerStay2D(Collider2D collider)
        {
            Debug.Log("Input_OnTriggerStay2D");
            if (collider.gameObject.CompareTag("Item"))
            {
                var collidedItem = collider.gameObject.GetComponent<ItemBehaviour>().item;
                if (collidedItem is null)
                {
                    Debug.LogError("Item on ground is null");
                    return;
                }

                if (!AcceptsItemCheck(collidedItem)) return;
                storage.AddItem(collidedItem);
                Destroy(collider.gameObject);
                if (CanCraftRecipe())
                {
                    CraftRecipe();
                }
            }
        }

        private bool AcceptsItemCheck(ItemData collidedItem)
        {
            var part = recipe?.parts.Find(part => part.item == collidedItem.itemName);
            return part is not null;
        }

        private bool CanCraftRecipe()
        {
            if (recipe == null) return false;

            foreach (var part in recipe.parts)
            {
                // Count the total number of the given item in storage
                int itemCount = 0;

                for (int i = 0; i < storage.maxItems; i++)
                {
                    var itemStack = storage.GetItemStack(i);
                    if (itemStack != null && itemStack.item.itemName == part.item)
                    {
                        itemCount += itemStack.amount;
                    }
                }

                // If any part is not satisfied, we cannot craft the recipe
                if (itemCount < part.quantity) // Assuming 1 needed, adjust if RecipePart has a count field
                {
                    return false;
                }
            }

            return true;
        }

        private void CraftRecipe()
        {
            // Check if the current recipe is not null
            if (recipe == null) return;

            // Iterate through each part required by the recipe
            foreach (var part in recipe.parts)
            {
                int countToRemove = part.quantity; // Assuming 'quantity' is a needed field here

                // Loop through the storage slots
                for (int i = 0; i < storage.maxItems && countToRemove > 0; i++)
                {
                    ItemStack stack = storage.GetItemStack(i);
                    if (stack != null && stack.item.itemName == part.item)
                    {
                        // Calculate how many items to remove
                        int removeCount = Mathf.Min(stack.amount, countToRemove);
                        stack.amount -= removeCount;
                        countToRemove -= removeCount;

                        // If the stack is empty now, remove the stack from the storage
                        if (stack.amount <= 0)
                        {
                            storage.RemoveItem(stack.item);
                        }
                    }
                }
            }

            // Assuming 'outputItemName' is in a format that can be used to create or fetch an ItemData instance
            var craftedItemData = itemRegistry.GetItem(recipe.outputItemName);

            // Spawn the crafted item, on the output (top) of the furnace, look out for rotation
            var prefab = craftedItemData.prefab;
            if (prefab is null)
            {
                Debug.LogError($"Item prefab for {craftedItemData.name} not found.");
                return;
            }
            // Calculate the position to instantiate based on current tile's orientation
            Vector3 spawnOffset = Vector3.zero;
            switch (orientation)
            {
                case Orientation.Up:
                    spawnOffset = new Vector3(0, 1, 0); // Move item to the top of the tile
                    break;
                case Orientation.Right:
                    spawnOffset = new Vector3(1, 0, 0); // Move item to the right of the tile
                    break;
                case Orientation.Down:
                    spawnOffset = new Vector3(0, -1, 0); // Move item to the bottom of the tile
                    break;
                case Orientation.Left:
                    spawnOffset = new Vector3(-1, 0, 0); // Move item to the left of the tile
                    break;
            }

            // Instantiate the prefab at the calculated position
            Vector3 spawnPosition = transform.position + spawnOffset;
            Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Optionally: Notify the player or system that crafting was successful
            Debug.Log($"Crafted item: {recipe.outputItemName}");
        }
    }
}