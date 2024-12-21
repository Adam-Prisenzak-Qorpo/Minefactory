using UnityEngine;
using UnityEngine.UI;
using Minefactory.World.Tiles.Behaviour;
using Minefactory.Game;
using Minefactory.Storage.Items;
using Minefactory.Storage;
using Minefactory.Factories;
using Minefactory.Factories.Mining;

using System.Collections;

namespace Minefactory.World.Tiles.Behaviour
{
    public class MinerOutputBehaviour : BreakableTileBehaviour
    {
        [Header("References")]
        public GameObject configUIPrefab;
        public ItemRegistry itemRegistry;

        [Header("Resource Settings")]
        private float lastIronSpawnTime = 0f;
        private float lastGoldSpawnTime = 0f;
        private float ironOutputRate = 0f;
        private float goldOutputRate = 0f;
        private const float SPAWN_HEIGHT = 1f;

        private GameObject activeConfigUI;
        private MinerOutputUI uiScript;

        public override bool CanBePlaced(Vector2 position)
        {
            // var worldManager = WorldManager.Instance;
            // if (!worldManager.GetActiveWorld().name.Contains("Top"))
            // {
            //     Debug.Log("Miner output can only be placed in top world");
            //     return false;
            // }
            return true;
        }

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            SpawnResources();
        }

        public float GetIronOutputRate()
        {
            return ironOutputRate;
        }

        public float GetGoldOutputRate()
        {
            return goldOutputRate;
        }

        protected override void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1)) // Right click
            {
                ShowConfigUI();
            }
        }

        private void ShowConfigUI()
        {
            Debug.Log("ShowConfigUI called");
            if (configUIPrefab != null && activeConfigUI == null)
            {
                Debug.Log("Attempting to create UI");
                // Find the Canvas in the scene
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    Debug.LogError("No canvas found in scene!");
                    return;
                }

                // Instantiate the UI
                activeConfigUI = Instantiate(configUIPrefab, canvas.transform);
                Debug.Log($"UI instantiated: {activeConfigUI != null}");
                
                uiScript = activeConfigUI.GetComponent<MinerOutputUI>();
                Debug.Log($"UI script found: {uiScript != null}");
                
                if (uiScript != null)
                {
                    uiScript.Initialize(this);
                }
            }

            if (activeConfigUI != null)
            {
                activeConfigUI.SetActive(true);
                if (uiScript != null)
                {
                    Debug.Log("Updating UI");
                    uiScript.UpdateUI();
                }
            }
        }

        public void CloseConfigUI()
        {
            if (activeConfigUI != null)
            {
                activeConfigUI.SetActive(false);
            }
        }

        private void SpawnResources()
        {
            float nextIronSpawnTime = lastIronSpawnTime + (60f / ironOutputRate);
            float nextGoldSpawnTime = lastGoldSpawnTime + (60f / goldOutputRate);

            if (ironOutputRate > 0 && Time.time >= nextIronSpawnTime)
            {
                SpawnResource("iron");
                lastIronSpawnTime = Time.time;
            }

            if (goldOutputRate > 0 && Time.time >= nextGoldSpawnTime)
            {
                SpawnResource("gold");
                lastIronSpawnTime = Time.time;
            }
        }

        private void SpawnResource(string resourceType)
        {
            ItemData resourceItem = itemRegistry.GetItem($"{resourceType}_raw");
            if (resourceItem != null && resourceItem.prefab != null)
            {
                Vector3 spawnPos = transform.position + new Vector3(0, SPAWN_HEIGHT, 0);
                GameObject spawnedItem = Instantiate(resourceItem.prefab, spawnPos, Quaternion.identity);
                var itemBehaviour = spawnedItem.GetComponent<ItemBehaviour>();
                if (itemBehaviour != null)
                {
                    itemBehaviour.item = resourceItem;
                }
            }
        }

        public void SetOutputRate(string resourceType, float rate)
        {
            var manager = MiningProductionManager.Instance;
            
            // Get the current rate for this specific miner
            float currentRate = (resourceType == "iron") ? ironOutputRate : goldOutputRate;
            
            // Calculate total production and current total output
            float totalProduction = manager.GetTotalProductionRate(resourceType);
            float currentTotalOutput = manager.GetCurrentOutputRate(resourceType);
            
            // Calculate the maximum allowed rate by considering the total production
            // and subtracting all output except this miner's current output
            float maxAllowedRate = totalProduction - (currentTotalOutput - currentRate);
            
            // Clamp the new rate
            float newRate = Mathf.Min(rate, maxAllowedRate);

            // Update the manager with the rate difference
            manager.SetOutputRate(resourceType, currentTotalOutput - currentRate + newRate);

            // Update local rate
            if (resourceType == "iron")
                ironOutputRate = newRate;
            else if (resourceType == "gold")
                goldOutputRate = newRate;
        }

        protected override void OnDestroy()
        {
            var manager = MiningProductionManager.Instance;
            if (manager != null)
            {
                manager.SetOutputRate("iron", manager.GetCurrentOutputRate("iron") - ironOutputRate);
                manager.SetOutputRate("gold", manager.GetCurrentOutputRate("gold") - goldOutputRate);
            }

            if (activeConfigUI != null)
            {
                Destroy(activeConfigUI);
            }
        }
    }
}