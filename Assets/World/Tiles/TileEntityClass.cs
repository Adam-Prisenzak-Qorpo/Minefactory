using UnityEngine;

public class TileEntityClass : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;

    void Start()
    {
        Debug.Log($"Tile Entity Class: {this.name}");
    }

    void OnMouseDown()
    {
        Debug.Log($"Clicked on {this.name}");
        Destroy(gameObject);
    }
}
