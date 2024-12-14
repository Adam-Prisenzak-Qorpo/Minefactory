using System;
using Minefactory.Factories;
using Minefactory.Factories.Recipes;
using Minefactory.Storage.Items;
using UnityEngine;

namespace Factories.Recipes
{
    public class MaterialRowBehaviour : MonoBehaviour
    {

        public GameObject textObject;
        public GameObject imgObject;
        public ItemRegistry itemRegistry;
        
        
        
        
        public void SetRecipe(RecipePart part)
        {
            textObject.GetComponent<UnityEngine.UI.Text>().text = part.FullName;
            imgObject.GetComponent<UnityEngine.UI.Image>().sprite = itemRegistry.GetItem(part.item).sprite;
        }

    }
}