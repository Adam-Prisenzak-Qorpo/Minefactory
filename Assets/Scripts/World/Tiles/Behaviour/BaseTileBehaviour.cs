using UnityEngine;
using Minefactory.Storage.Items;
using Minefactory.Common;
using Minefactory.Game;

namespace Minefactory.World.Tiles.Behaviour
{
    public abstract class BaseTileBehaviour : MonoBehaviour
    {
        public ItemData item;
        public Orientation orientation = Orientation.Up;
        public GameState gameState;

        public bool isGhostTile = false;

        protected virtual void Start()
        {
            if (item == null)
            {
                Debug.LogWarning("No item data assigned to tile behaviour");
            }
        }

        public virtual bool CanBePlaced(Vector2 position)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(position);
            
            foreach (Collider2D collider in colliders)
            {
                // Skip if the collider belongs to any object in our parent hierarchy
                if (transform.IsChildOf(collider.transform))
                {
                    continue;
                }

                if (collider.CompareTag("Solid"))
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual void OnMouseOver()
        {}

        protected virtual void OnDestroy()
        {}

        protected bool IsSolidTileAt(Vector2 position)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(position);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Solid") && collider.gameObject != gameObject)
                {
                    return true;
                }
            }
            return false;
        }

        protected T FindTileAtPosition<T>(Vector2 position) where T : BaseTileBehaviour
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(position);
            foreach (Collider2D collider in colliders)
            {
                T behaviour = collider.GetComponent<T>();
                if (behaviour != null)
                {
                    return behaviour;
                }
            }
            return null;
        }
    }
}