using Minefactory.Common;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.Common.Behaviour;
using Minefactory.Factories.Recipes;
using UnityEngine;
using Minefactory.Factories;
using Minefactory.Game;
using System.Collections.Generic;

namespace Minefactory.World.Tiles.Behaviour
{
    public class CrafterTileBehaviour : BreakableTileBehaviour
    {
        public ItemRegistry itemRegistry;

        private StorageData storage;
        private Collision2DProxy inputCollider;
        private Collision2DProxy outputCollider;
        private ItemRecipe currentRecipe;
        private GameObject crafterUI;
        private bool isHovered = false;

        void Start()
        {
            crafterUI = WorldManager.Instance.GetUIManager().crafterUI;
            storage = ScriptableObject.CreateInstance<StorageData>();

            inputCollider = transform.Find("Input").GetComponent<Collision2DProxy>();
            if (inputCollider is null)
            {
                Debug.LogError("Input collider is null");
                return;
            }
            inputCollider.OnTriggerStay2D_Action += Input_OnTriggerStay2D;

            outputCollider = transform.Find("Output").GetComponent<Collision2DProxy>();
            if (outputCollider is null)
            {
                Debug.LogError("Output collider is null");
            }

            // Load saved recipe from metadata if it exists
            LoadRecipeFromMetadata();
        }

        private void LoadRecipeFromMetadata()
        {
            var modManager = WorldManager.activeBaseWorld.GetComponent<WorldModificationManager>();
            var metadata = modManager.GetModificationMetadata(transform.position);

            if (metadata != null && metadata.ContainsKey("recipeOutput"))
            {
                string recipeName = metadata["recipeOutput"];
                var gameState = GetComponent<GameState>() ?? FindObjectOfType<GameState>();
                if (gameState != null)
                {
                    currentRecipe = gameState.itemRecipes.Find(r => r.outputItemName == recipeName && r.type == RecipeType.Crafting);
                }
            }
        }

        private void SaveRecipeToMetadata()
        {
            if (currentRecipe != null)
            {
                var modManager = WorldManager.activeBaseWorld.GetComponent<WorldModificationManager>();
                var metadata = modManager.GetModificationMetadata(transform.position) ?? new Dictionary<string, string>();
                
                metadata["recipeOutput"] = currentRecipe.outputItemName;
                
                var tileRegistry = WorldManager.activeBaseWorld.tileRegistry;
                var tileData = tileRegistry.GetTileByItem(item);
                modManager.SetModification(transform.position, tileData, orientation, metadata);
            }
        }

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
            if (mouseDown && isHovered && !crafterUI.activeSelf)
            {
                var script = crafterUI.GetComponent<CrafterBehaviour>();
                script.ClearListeners();
                script.SelectRecipe(currentRecipe);
                script.selectRecipeEvent.AddListener(OnSelectRecipe);
                script.currentCrafter = this;
                WorldManager.Instance.GetUIManager().OpenCrafterUI();
            }
        }

        private void OnSelectRecipe(ItemRecipe newRecipe)
        {
            currentRecipe = newRecipe;
            SaveRecipeToMetadata();
        }

        private void Input_OnTriggerStay2D(Collider2D collider)
        {
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
            if (currentRecipe == null) return false;
            var part = currentRecipe.parts.Find(part => part.item == collidedItem.itemName);
            return part != null;
        }

        private bool CanCraftRecipe()
        {
            if (currentRecipe == null) return false;

            foreach (var part in currentRecipe.parts)
            {
                int itemCount = 0;
                for (int i = 0; i < storage.maxItems; i++)
                {
                    var itemStack = storage.GetItemStack(i);
                    if (itemStack != null && itemStack.item.itemName == part.item)
                    {
                        itemCount += itemStack.amount;
                    }
                }

                if (itemCount < part.quantity)
                {
                    return false;
                }
            }
            return true;
        }

        private void CraftRecipe()
        {
            if (currentRecipe == null) return;

            foreach (var part in currentRecipe.parts)
            {
                int countToRemove = part.quantity;
                for (int i = 0; i < storage.maxItems && countToRemove > 0; i++)
                {
                    ItemStack stack = storage.GetItemStack(i);
                    if (stack != null && stack.item.itemName == part.item)
                    {
                        int removeCount = Mathf.Min(stack.amount, countToRemove);
                        stack.amount -= removeCount;
                        countToRemove -= removeCount;

                        if (stack.amount <= 0)
                        {
                            storage.RemoveItem(stack.item);
                        }
                    }
                }
            }

            var craftedItemData = itemRegistry.GetItem(currentRecipe.outputItemName);
            if (craftedItemData == null || craftedItemData.prefab == null)
            {
                Debug.LogError($"Item prefab for {currentRecipe.outputItemName} not found.");
                return;
            }

            Vector3 spawnOffset = GetSpawnOffset();
            Vector3 spawnPosition = transform.position + spawnOffset;
            var spawnedItem = Instantiate(craftedItemData.prefab, spawnPosition, Quaternion.identity);
            spawnedItem.GetComponent<ItemBehaviour>().item = craftedItemData;

            Debug.Log($"Crafted item: {currentRecipe.outputItemName}");
        }

        private Vector3 GetSpawnOffset()
        {
            return orientation switch
            {
                Orientation.Up => new Vector3(0, 1, 0),
                Orientation.Right => new Vector3(1, 0, 0),
                Orientation.Down => new Vector3(0, -1, 0),
                Orientation.Left => new Vector3(-1, 0, 0),
                _ => Vector3.zero
            };
        }
    }
}