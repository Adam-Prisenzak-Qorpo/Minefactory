using UnityEngine;
using Minefactory.Game;
using Minefactory.Factories.Mining;
using System.Collections.Generic;
using System.Collections;

namespace Minefactory.World.Tiles.Behaviour
{
    public class AutominerBehaviour : PersistentTileBehaviour
    {

        [Header("Mining Settings")]
        [SerializeField] private float baseMiningRate = 1f;
        private string currentOreType;
        private bool isActive = false;

        public override bool CanBePlaced(Vector2 position)
        {
            var worldManager = WorldManager.Instance;
            if (worldManager.GetActiveWorld().name.Contains("Top"))
            {
                Debug.Log("Autominer can only be placed in underground world");
                return false;
            }

            if (!isGhostTile)
            {
                var activeWorld = WorldManager.activeBaseWorld;
                var modManager = activeWorld.GetComponent<WorldModificationManager>();
                var savedMetadata = modManager.GetModificationMetadata(position);
                
                if (savedMetadata != null && savedMetadata.ContainsKey("oreType"))
                {
                    currentOreType = savedMetadata["oreType"];
                    return true;
                }
            }

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
            
            if (!isGhostTile)
            {
                var modManager = WorldManager.activeBaseWorld.GetComponent<WorldModificationManager>();
                var savedMetadata = modManager.GetModificationMetadata(transform.position);
                
                if (savedMetadata != null && savedMetadata.ContainsKey("oreType"))
                {
                    currentOreType = savedMetadata["oreType"];
                }
                
                // Set metadata including ore type
                var metadata = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(currentOreType))
                {
                    metadata["oreType"] = currentOreType;
                }

                var tileRegistry = WorldManager.activeBaseWorld.tileRegistry;
                var tileData = tileRegistry.GetTileByItem(item);
                modManager.SetModification(transform.position, tileData, orientation, metadata);

                OnActivate();
            }
        }

        public override void OnActivate()
        {
            Debug.Log($"Autominer OnActivate called at {transform.position} with oreType: {currentOreType}");
            base.OnActivate();
            if (!string.IsNullOrEmpty(currentOreType) && !isActive)
            {
                MiningProductionManager.Instance.AddMiningProduction(currentOreType, baseMiningRate);
                isActive = true;
                Debug.Log($"Autominer activated and registered production for {currentOreType}");
            }
        }

        public override void OnDeactivate()
        {
            if (isActive && !string.IsNullOrEmpty(currentOreType))
            {
                MiningProductionManager.Instance.RemoveMiningProduction(currentOreType, baseMiningRate);
                isActive = false;
            }
            base.OnDeactivate();
        }
    }
}