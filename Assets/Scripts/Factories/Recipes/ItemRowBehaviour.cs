using System;
using Minefactory.Factories;
using Minefactory.Factories.Recipes;
using Minefactory.Storage.Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Factories.Recipes
{
    public class ItemRowBehaviour : MonoBehaviour, IPointerDownHandler
    {

        public GameObject textObject;
        public GameObject imgObject;
        public ItemRegistry itemRegistry;
        
        public SelectRecipeEvent selectRecipeEvent;
        
        private ItemRecipe recipe;

        public void Start()
        {
            selectRecipeEvent ??= new SelectRecipeEvent();
        }


        public void SetRecipe(ItemRecipe newRecipe)
        {
            recipe = newRecipe;
            textObject.GetComponent<UnityEngine.UI.Text>().text = recipe.outputItemName;
            imgObject.GetComponent<UnityEngine.UI.Image>().sprite = itemRegistry.GetItem(recipe.outputItemName).sprite;
        }

        public void OnPointerDown (PointerEventData eventData)
        {
            selectRecipeEvent?.Invoke(recipe);
            Debug.Log("Clicked");
        }
    }
}