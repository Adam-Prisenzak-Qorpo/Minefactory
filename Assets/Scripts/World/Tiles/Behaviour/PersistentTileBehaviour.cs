using UnityEngine;
using Minefactory.Game;
using System.Collections.Generic;

namespace Minefactory.World.Tiles.Behaviour
{
    public abstract class PersistentTileBehaviour : BreakableTileBehaviour
    {
        protected bool isPersistentActive = false;
        protected bool isBeingDestroyed = false;

        protected override void Start()
        {
            base.Start();
            if (!isGhostTile)
            {
                var modManager = WorldManager.activeBaseWorld.GetComponent<WorldModificationManager>();
      
                modManager.RegisterPersistentTile(transform.position, this);

                var existingMetadata = modManager.GetModificationMetadata(transform.position) 
                    ?? new Dictionary<string, string>();

                existingMetadata["isPersistent"] = "true";

                var tileRegistry = WorldManager.activeBaseWorld.tileRegistry;
                var tileData = tileRegistry.GetTileByItem(item);
                modManager.SetModification(transform.position, tileData, orientation, existingMetadata);

            }
        }

        protected override void OnDestroy()
        {
            isBeingDestroyed = true;
            if (WorldManager.activeBaseWorld != null)
            {
                var modManager = WorldManager.activeBaseWorld.GetComponent<WorldModificationManager>();
                if (modManager != null)
                {
                    if (!isGhostTile && WorldManager.activeBaseWorld.onTileRemoved != null)
                    {
                        modManager.UnregisterPersistentTile(transform.position);
                        modManager.ClearModification(transform.position);
                    }
                    
                    if (isPersistentActive)
                    {
                        OnDeactivate();
                    }
                }
            }
            
            base.OnDestroy();
        }

        public virtual void OnActivate()
        {
            isPersistentActive = true;
            Debug.Log($"Persistent tile activated at position {transform.position}");
        }

        public virtual void OnDeactivate()
        {
            isPersistentActive = false;
            Debug.Log($"Persistent tile deactivated at position {transform.position}");
        }

        public virtual void PersistentUpdate()
        {}
    }
}