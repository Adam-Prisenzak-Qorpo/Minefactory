using Minefactory.Storage;
using Minefactory.Storage.Items;
using Minefactory.Common.Behaviour;
using UnityEngine;
namespace Minefactory.World.Tiles.Behaviour
{
    public class FurnaceTileBehaviour : BreakableTileBehaviour
    {
        private StorageData storage;

        private Collision2DProxy inputCollider;
        private Collision2DProxy outputCollider;

        void Start()
        {
            storage = ScriptableObject.CreateInstance<StorageData>();

            inputCollider = transform.Find("Input").GetComponent<Collision2DProxy>();
            if (inputCollider is null)
            {
                Debug.LogError("Input collider is null");
                return;
            }
            inputCollider.OnTriggerStay2D_Action += Input_OnTriggerStay2D;

            outputCollider = transform.Find("Output").GetComponent<Collision2DProxy>();
            if (outputCollider is null)
            {
                Debug.LogError("Output collider is null");
                return;
            }
        }

        private void Input_OnTriggerStay2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Item"))
            {
                var item = collider.gameObject.GetComponent<ItemBehaviour>().item;
                if (item is null)
                {
                    Debug.LogError("Item on ground is null");
                    return;
                }
                storage.AddItem(item);
                Destroy(collider.gameObject);
            }
        }
    }
}