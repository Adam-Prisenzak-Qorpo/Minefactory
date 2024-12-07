using System;
using Factories.Recipes;
using Minefactory.Factories.Recipes;
using Minefactory.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Minefactory.Factories
{
    [Serializable]
    public class SelectRecipeEvent: UnityEvent<ItemRecipe> { }

    public class ItemListBehaviour: MonoBehaviour
    {
        public GameState gameState;
        public GameObject listContainerPrefab;
        public GameObject materialListContainer;
        
        public SelectRecipeEvent selectRecipeEvent;
        
        void Start()
        {
            selectRecipeEvent ??= new SelectRecipeEvent();
            foreach (var item in gameState.itemRecipes)
            {
                var recipeContainer = Instantiate(listContainerPrefab, transform);
                // Fit to parent scale
                recipeContainer.transform.localScale = Vector3.one;

                var script = recipeContainer.GetComponent<ItemRowBehaviour>();
                script.SetRecipe(item);
                script.selectRecipeEvent.AddListener(OnSelectRecipe); 
            }
        }
        
        private void OnSelectRecipe(ItemRecipe recipe)
        {
            selectRecipeEvent?.Invoke(recipe);
        }
    }
}