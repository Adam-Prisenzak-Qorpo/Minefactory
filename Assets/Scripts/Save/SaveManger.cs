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
using System.Collections.Generic;

namespace Minefactory.Save
{
    [Serializable]
    public class PlayerSaveData
    {
        public float positionX;
        public float positionY;
        public bool isInTopWorld;
    }

    [Serializable]
    public class WorldSaveData
    {
        public float seed;
        public List<ChunkData> topWorldModifications = new List<ChunkData>();
        public List<ChunkData> undergroundWorldModifications = new List<ChunkData>();
    }


    [Serializable]
    public class GameSaveData
    {
        public PlayerSaveData playerData;
        public WorldSaveData worldData;
        public string saveDateTime;
    }

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

                if (topWorldActive == true){
                    player = topWorld.GetComponentInChildren<PlayerController>();
                    worldGen = topWorld.GetComponentInChildren<BaseWorldGeneration>();
                }
                else{
                    player = undergroundWorld.GetComponentInChildren<PlayerController>();
                    worldGen = undergroundWorld.GetComponentInChildren<BaseWorldGeneration>();
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
                PlayerController player = null;
                if(saveData.playerData.isInTopWorld){
                    player = topWorld.GetComponentInChildren<PlayerController>();
                }
                else{
                    player = undergroundWorld.GetComponentInChildren<PlayerController>();
                }
                player.transform.position = new Vector2(
                    saveData.playerData.positionX,
                    saveData.playerData.positionY
                );
                BaseWorldGeneration topWorldGen = topWorld.GetComponentInChildren<BaseWorldGeneration>();
                BaseWorldGeneration underWorldGen = undergroundWorld.GetComponentInChildren<BaseWorldGeneration>();
                topWorldGen.InitializeWorld(saveData.worldData.seed, saveData.worldData.topWorldModifications);
                underWorldGen.InitializeWorld(saveData.worldData.seed, saveData.worldData.undergroundWorldModifications);

                if (saveData.playerData.isInTopWorld)
                {
                    topWorld.SetActive(true);
                }
                else
                {
                    undergroundWorld.SetActive(true);
                }
                Debug.Log("Game loaded successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading game: {e.Message}\n{e.StackTrace}");
            }
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