using Minefactory.Common;
using Unity.VisualScripting;
using UnityEngine;
namespace Minefactory.World.Tiles.Behaviour
{
    public class BeltTileBehaviour : BreakableTileBehaviour
    {
        // Check for collision with player and add force to player


        void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player is on belt");
                Rigidbody2D rb = collider.gameObject.GetComponent<Rigidbody2D>();
                var velocity = orientation switch
                {
                    Orientation.Up => new Vector2(0, 1),
                    Orientation.Down => new Vector2(0, -1),
                    Orientation.Left => new Vector2(-1, 0),
                    Orientation.Right => new Vector2(1, 0),
                    _ => new Vector2(0, 0)
                };
                Debug.Log(rb.position);
                rb.position += velocity * 0.01f;

            }
        }
    }
}