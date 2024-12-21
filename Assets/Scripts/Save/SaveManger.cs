using UnityEngine;
using System;
using System.IO;
using Minefactory.Player;
using Minefactory.Player.Inventory;
using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.World;
using Minefactory.World.Tiles;
using Minefactory.Common;
using Minefactory.World.Tiles.Behaviour;
using System.Collections.Generic;



namespace Minefactory.Save
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager instance;
        private GameObject topWorld;
        private GameObject undergroundWorld;
        public static SaveManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("SaveManager");
                    instance = go.AddComponent<SaveManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private string SavePath => Path.Combine(Application.persistentDataPath, "game_save.json");

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SaveGame()
        {
            try
            {
                bool topWorldActive = topWorld.activeSelf;
                PlayerController player = null;
                BaseWorldGeneration worldGen = null;
                Inventory inventory = null;

                if (topWorldActive)
                {
                    player = topWorld.GetComponentInChildren<PlayerController>();
                    worldGen = topWorld.GetComponentInChildren<BaseWorldGeneration>();
                    inventory = topWorld.GetComponentInChildren<Inventory>();
                }
                else
                {
                    player = undergroundWorld.GetComponentInChildren<PlayerController>();
                    worldGen = undergroundWorld.GetComponentInChildren<BaseWorldGeneration>();
                    inventory = undergroundWorld.GetComponentInChildren<Inventory>();
                }

                var inventoryData = new InventoryData();
                if (inventory != null && inventory.inventoryData != null)
                {
                    for (int i = 0; i < inventory.inventoryData.maxItems; i++)
                    {
                        var itemStack = inventory.inventoryData.GetItemStack(i);
                        if (itemStack != null && itemStack.item != null && itemStack.amount > 0)
                        {
                            inventoryData.items.Add(new InventoryItemData(
                                itemStack.item.itemName,
                                itemStack.amount
                            ));
                        }
                    }
                }

                GameSaveData saveData = new GameSaveData
                {
                    playerData = new PlayerSaveData
                    {
                        positionX = player.transform.position.x,
                        positionY = player.transform.position.y,
                        isInTopWorld = topWorldActive
                    },
                    worldData = new WorldSaveData
                    {
                        seed = worldGen.getSeed(),
                        topWorldModifications = topWorld.GetComponentInChildren<WorldModificationManager>().GetModifications(),
                        undergroundWorldModifications = undergroundWorld.GetComponentInChildren<WorldModificationManager>().GetModifications()
                    },
                    inventoryData = inventoryData,
                    saveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SavePath, json);
                Debug.Log($"Game saved successfully to {SavePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving game: {e.Message}\n{e.StackTrace}");
            }
        }

        public void LoadGame(GameObject topWorld, GameObject undergroundWorld)
        {
            Debug.Log("Loading game... save path: " + SavePath);
            if (!File.Exists(SavePath))
            {
                Debug.Log("No save file found.");
                this.CreateNewGame(topWorld, undergroundWorld);
                return;
            }

            try
            {
                this.topWorld = topWorld;
                this.undergroundWorld = undergroundWorld;
                string json = File.ReadAllText(SavePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                // First initialize underground world (where autominers are)
                undergroundWorld.SetActive(true);
                BaseWorldGeneration underWorldGen = undergroundWorld.GetComponentInChildren<BaseWorldGeneration>();
                underWorldGen.InitializeWorld(saveData.worldData.seed, saveData.worldData.undergroundWorldModifications);

                // this could be done better but this is simple solution

                var autominers = undergroundWorld.GetComponentsInChildren<AutominerBehaviour>();
                foreach (var miner in autominers)
                {
                    if (!miner.isGhostTile)
                    {
                        miner.OnActivate();
                    }
                }

                

                topWorld.SetActive(true);
                TopWorldGeneration topWorldGen = topWorld.GetComponentInChildren<TopWorldGeneration>();
                topWorldGen.InitializeWorld(saveData.worldData.seed, saveData.worldData.topWorldModifications);

                Inventory inventory = topWorldGen.playerInventory;

                LoadInventory(saveData, inventory);

                PlayerController player = null;
                if (saveData.playerData.isInTopWorld)
                {
                    undergroundWorld.SetActive(false);
                    player = topWorld.GetComponentInChildren<PlayerController>();
                    topWorld.SetActive(true);
                }
                else
                {
                    topWorld.SetActive(false);
                    player = undergroundWorld.GetComponentInChildren<PlayerController>();
                    undergroundWorld.SetActive(true);
                }

                player.transform.position = new Vector2(
                    saveData.playerData.positionX,
                    saveData.playerData.positionY
                );

                Debug.Log("Game loaded successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading game: {e.Message}\n{e.StackTrace}");
            }
        }

        private void LoadInventory(GameSaveData saveData, Inventory inventory)
        {
            if (saveData.inventoryData == null)
            {
                return;
            }

            if (inventory == null || inventory.inventoryData == null)
            {
                Debug.LogError("Could not find valid inventory to load into");
                return;
            }

            foreach (var itemData in saveData.inventoryData.items)
            {
                var item = inventory.itemRegistry.GetItem(itemData.itemName);
                if (item != null)
                {
                    for (int i = 0; i < itemData.amount; i++)
                    {
                        inventory.inventoryData.AddItem(item);
                    }
                }
            }
            inventory.UpdateUI();
        }
        public void CreateNewGame(GameObject topWorld, GameObject undergroundWorld)
        {
            try
            {
                this.topWorld = topWorld;
                this.undergroundWorld = undergroundWorld;
                BaseWorldGeneration topWorldGen = topWorld.GetComponentInChildren<BaseWorldGeneration>();
                BaseWorldGeneration underWorldGen = undergroundWorld.GetComponentInChildren<BaseWorldGeneration>();
                float seed = UnityEngine.Random.Range(0, 1000000);
                topWorldGen.InitializeWorld(seed);
                underWorldGen.InitializeWorld(seed);
                topWorld.SetActive(false);
                undergroundWorld.SetActive(true);
                Debug.Log("New game created successfully with seed: " + seed);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating new game: {e.Message}\n{e.StackTrace}");
            }
        }


        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}