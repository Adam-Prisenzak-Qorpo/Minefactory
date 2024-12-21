using UnityEngine;
using UnityEngine.UI;
using Minefactory.World.Tiles.Behaviour;
using Minefactory.Game;
using Minefactory.Storage.Items;
using Minefactory.Factories;
using Minefactory.Factories.Mining;
using System.Collections;
using System.Collections.Generic;

namespace Minefactory.World.Tiles.Behaviour
{
    public class AutominerBehaviour : BreakableTileBehaviour
    {
        private const float BASE_MINING_RATE = 1f; // 1 ore per minute
        private string currentOreType;
        private bool isActive = false;

    public override bool CanBePlaced(Vector2 position)
    {
        // Check if we're in the underground world
        var worldManager = WorldManager.Instance;
        if (worldManager.GetActiveWorld().name.Contains("Top"))
        {
            Debug.Log("Autominer can only be placed in underground world");
            return false;
        }

        // Only check saved metadata if this is not a ghost tile
        if (!isGhostTile)
        {
            var activeWorld = WorldManager.activeBaseWorld;
            var modManager = activeWorld.GetComponent<WorldModificationManager>();
            var savedMetadata = modManager.GetModificationMetadata(position);
            Debug.Log($"[Real Placement] Checking for saved metadata at position {position}: {(savedMetadata != null ? "found" : "not found")}");
            
            if (savedMetadata != null && savedMetadata.ContainsKey("oreType"))
            {
                currentOreType = savedMetadata["oreType"];
                Debug.Log($"[Real Placement] Found saved ore type: {currentOreType}");
                return true;
            }
        }

        // Original ore detection logic
        bool foundOre = false;
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);
        foreach (Collider2D collider in colliders)
        {
            var tileBehaviour = collider.GetComponent<BaseTileBehaviour>();
            if (tileBehaviour != null && tileBehaviour.item != null)
            {
                string itemName = tileBehaviour.item.itemName.ToLower();
                if (itemName.Contains("iron") || itemName.Contains("gold"))
                {
                    currentOreType = itemName.Contains("iron") ? "iron" : "gold";
                    foundOre = true;

                    // Only store metadata if this is not a ghost tile
                    if (!isGhostTile)
                    {
                        // Store the ore type in metadata
                        var newMetadata = new Dictionary<string, string> { { "oreType", currentOreType } };
                        Debug.Log($"[Real Placement] Setting metadata for position {position} with ore type: {currentOreType}");
                        
                        var activeWorld = WorldManager.activeBaseWorld;
                        var modManager = activeWorld.GetComponent<WorldModificationManager>();
                        var tileRegistry = activeWorld.tileRegistry;
                        var tileData = tileRegistry.GetTileByItem(item);
                        modManager.SetModification(position, tileData, orientation, newMetadata);
                    }
                    break;
                }
            }
        }

        if (!foundOre)
        {
            Debug.Log("Autominer must be placed on an ore");
            return false;
        }

        return true;
    }

        protected override void Start()
        {
            base.Start();
            
            // Try to get ore type from metadata if it wasn't set during placement
            if (string.IsNullOrEmpty(currentOreType))
            {
                var modManager = WorldManager.activeBaseWorld.GetComponent<WorldModificationManager>();
                var savedMetadata = modManager.GetModificationMetadata(transform.position);
                if (savedMetadata != null && savedMetadata.ContainsKey("oreType"))
                {
                    currentOreType = savedMetadata["oreType"];
                    ActivateMiner();
                }
                else
                {
                    DetectOreType();
                }
            }
            else
            {
                ActivateMiner();
            }
        }

        private void DetectOreType()
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
            foreach (Collider2D collider in colliders)
            {
                var tileBehaviour = collider.GetComponent<BaseTileBehaviour>();
                if (tileBehaviour != null && tileBehaviour.item != null)
                {
                    string itemName = tileBehaviour.item.itemName.ToLower();
                    if (itemName.Contains("iron"))
                    {
                        currentOreType = "iron";
                        break;
                    }
                    else if (itemName.Contains("gold"))
                    {
                        currentOreType = "gold";
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(currentOreType))
            {
                // Store the detected ore type in metadata
                var modManager = WorldManager.activeBaseWorld.GetComponent<WorldModificationManager>();
                var newMetadata = new Dictionary<string, string> { { "oreType", currentOreType } };
                var tileRegistry = WorldManager.activeBaseWorld.tileRegistry;
                var tileData = tileRegistry.GetTileByItem(item);
                modManager.SetModification(transform.position, tileData, orientation, newMetadata);
                
                ActivateMiner();
            }
        }

        private void ActivateMiner()
        {
            if (!isActive && !string.IsNullOrEmpty(currentOreType))
            {
                MiningProductionManager.Instance.AddMiningProduction(currentOreType, BASE_MINING_RATE);
                isActive = true;
                Debug.Log($"Activated autominer for {currentOreType} at position {transform.position}");
            }
        }

        protected override void OnDestroy()
        {
            if (isActive && !string.IsNullOrEmpty(currentOreType))
            {
                MiningProductionManager.Instance.RemoveMiningProduction(currentOreType, BASE_MINING_RATE);
            }
            base.OnDestroy();
        }
    }
}