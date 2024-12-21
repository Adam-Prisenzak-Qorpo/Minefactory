using UnityEngine;
using UnityEngine.UI;
using Minefactory.World.Tiles.Behaviour;
using Minefactory.Game;
using Minefactory.Storage.Items;
using Minefactory.Storage;
using Minefactory.Factories;
using Minefactory.Factories.Mining;
using System.Collections.Generic;

namespace Minefactory.World.Tiles.Behaviour
{
    public class MinerOutputBehaviour : PersistentTileBehaviour
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

        protected override void Start()
        {
            base.Start();

            if (!isGhostTile)
            {
                var modManager = WorldManager.activeBaseWorld.GetComponent<WorldModificationManager>();
                var savedMetadata = modManager.GetModificationMetadata(transform.position);

                if (savedMetadata != null)
                {
                    if (savedMetadata.TryGetValue("ironOutputRate", out string ironRate))
                    {
                        SetOutputRate("iron", float.Parse(ironRate));
                    }
                    if (savedMetadata.TryGetValue("goldOutputRate", out string goldRate))
                    {
                        SetOutputRate("gold", float.Parse(goldRate));
                    }
                }
            }
        }

        public override void OnActivate()
        {
            base.OnActivate();
            var manager = MiningProductionManager.Instance;
            
            if (ironOutputRate > 0)
            {
                manager.SetOutputRate("iron", manager.GetCurrentOutputRate("iron") + ironOutputRate);
            }
            if (goldOutputRate > 0)
            {
                manager.SetOutputRate("gold", manager.GetCurrentOutputRate("gold") + goldOutputRate);
            }
        }

        public override void OnDeactivate()
        {
            var manager = MiningProductionManager.Instance;
            if (manager != null)
            {
                if (ironOutputRate > 0)
                {
                    manager.SetOutputRate("iron", manager.GetCurrentOutputRate("iron") - ironOutputRate);
                }
                if (goldOutputRate > 0)
                {
                    manager.SetOutputRate("gold", manager.GetCurrentOutputRate("gold") - goldOutputRate);
                }
            }
            base.OnDeactivate();
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
            if (configUIPrefab != null && activeConfigUI == null)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    Debug.LogError("No canvas found in scene!");
                    return;
                }

                activeConfigUI = Instantiate(configUIPrefab, canvas.transform);
                uiScript = activeConfigUI.GetComponent<MinerOutputUI>();
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
                lastGoldSpawnTime = Time.time;
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
            float currentRate = (resourceType == "iron") ? ironOutputRate : goldOutputRate;
            float totalProduction = manager.GetTotalProductionRate(resourceType);
            float currentTotalOutput = manager.GetCurrentOutputRate(resourceType);
            float maxAllowedRate = totalProduction - (currentTotalOutput - currentRate);
            float newRate = Mathf.Min(rate, maxAllowedRate);
            manager.SetOutputRate(resourceType, currentTotalOutput - currentRate + newRate);

            if (resourceType == "iron")
                ironOutputRate = newRate;
            else if (resourceType == "gold")
                goldOutputRate = newRate;

            SaveRatesToMetadata();
        }

        private void SaveRatesToMetadata()
        {
            var modManager = WorldManager.activeBaseWorld.GetComponent<WorldModificationManager>();
            var existingMetadata = modManager.GetModificationMetadata(transform.position) 
                ?? new Dictionary<string, string>();

            existingMetadata["ironOutputRate"] = ironOutputRate.ToString();
            existingMetadata["goldOutputRate"] = goldOutputRate.ToString();

            var tileRegistry = WorldManager.activeBaseWorld.tileRegistry;
            var tileData = tileRegistry.GetTileByItem(item);
            modManager.SetModification(transform.position, tileData, orientation, existingMetadata);
        }

        protected override void OnDestroy()
        {
            if (activeConfigUI != null)
            {
                Destroy(activeConfigUI);
            }

            base.OnDestroy();
        }
    }
}