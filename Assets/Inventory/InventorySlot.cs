using Unity.VisualScripting;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public InventoryClass inventory;
    public ItemStack stack;
    private bool selected = false;
    private bool canPlace = true;

    // Start is called before the first frame update
    void Start()
    {
        var itemRenderer = this.AddComponent<SpriteRenderer>();
        itemRenderer.sprite = stack.item.sprite;
        itemRenderer.sortingOrder = 2;

        // Add text
        var textObject = new GameObject("Text");
        textObject.transform.parent = transform;
        textObject.transform.localScale = new Vector2(0.1f, 0.1f);
        textObject.transform.localPosition = new Vector2(-0.25f, 0.25f);

        var text = textObject.AddComponent<TextMesh>();
        text.text = stack.amount.ToString();
        text.fontSize = 100;
        text.characterSize = 0.3f;
        text.anchor = TextAnchor.MiddleCenter;
        text.color = Color.white;

        var textRenderer = textObject.GetComponent<Renderer>();
        textRenderer.sortingOrder = 3;

        var collider = this.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1, 1);
        collider.isTrigger = true;
    }

    void Update()
    {
        if (selected)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool unselect = Input.GetKey(KeyCode.Escape);
            if (unselect)
            {
                transform.localPosition = Vector2.zero;
                selected = false;
            }
        }
    }

    Vector2 TransformedPosition => transform.position - new Vector3(.5f, -.5f);

    void OnMouseOver()
    {
        if (selected)
        {
            var sprite = GetComponent<SpriteRenderer>();
            Debug.Log(WorldGeneration.canPlace(TransformedPosition));
            if (!WorldGeneration.canPlace(TransformedPosition))
            {
                sprite.color = Color.red;
                canPlace = false;
            }
            else
            {
                sprite.color = Color.white;
                canPlace = true;
            }
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"Clicked on {name}");
        // Bind position on mouse
        if (!selected)
        {
            selected = true;
        }
        else if (canPlace)
        {
            var didPlace = WorldGeneration.onTilePlaced(TransformedPosition, stack.item);
            if (!didPlace)
            {
                return;
            }
            var amount = inventory.RemoveItem(stack.item);
            if (amount <= 0)
            {
                Destroy(gameObject);
                return;
            }
            selected = false;
        }
    }
}