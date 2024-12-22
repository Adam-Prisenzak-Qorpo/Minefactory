using Factories.Recipes;
using Minefactory.Factories.Recipes;
using Minefactory.Game;
using UnityEngine;
using Minefactory.World.Tiles.Behaviour;

namespace Minefactory.Factories
{
    public class CrafterBehaviour : MonoBehaviour
    {
        public GameObject itemListContainer;
        public GameObject materialListContainer;
        public SelectRecipeEvent selectRecipeEvent;
        
        [HideInInspector]
        public CrafterTileBehaviour currentCrafter;
        
        private ItemRecipe recipe;

        void Start()
        {
            selectRecipeEvent ??= new SelectRecipeEvent();
            gameObject.SetActive(!gameObject.activeSelf);
            
            var script = itemListContainer.GetComponent<ItemListBehaviour>(); 
            script.selectRecipeEvent.AddListener(OnSelectRecipe);
        }

        void Update()
        {
            var escPressed = Input.GetKeyDown(KeyCode.Escape);
            if (escPressed && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                ClearListeners();
            }
        }

        void OnDisable()
        {
            ClearListeners();
        }

        public void ClearListeners()
        {
            selectRecipeEvent.RemoveAllListeners();
            currentCrafter = null;
        }

        public void OnSelectRecipe(ItemRecipe newRecipe)
        {
            if (currentCrafter != null)
            {
                Debug.Log($"Setting recipe {newRecipe.outputItemName} for crafter at {currentCrafter.transform.position}");
                SelectRecipe(newRecipe);
                selectRecipeEvent?.Invoke(recipe);
            }
        }

        public void SelectRecipe(ItemRecipe newRecipe)
        {
            recipe = newRecipe;
            var script = materialListContainer.GetComponent<MaterialListBehaviour>();
            script.SetRecipe(recipe);
        }
    }
}