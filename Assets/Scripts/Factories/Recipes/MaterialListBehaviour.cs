using Minefactory.Factories.Recipes;
using UnityEngine;

namespace Factories.Recipes
{
    public class MaterialListBehaviour : MonoBehaviour
    {
        public GameObject materialRowPrefab;

        public void SetRecipe(ItemRecipe recipe)
        {
            // Remove all child
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            if (recipe is null) return;
            foreach (var item in recipe.parts)
            {
                var recipeContainer = Instantiate(materialRowPrefab, transform);
                // Fit to parent scale
                recipeContainer.transform.localScale = Vector3.one;

                var script = recipeContainer.GetComponent<MaterialRowBehaviour>();
                script.SetRecipe(item);
            }
        }
        
    }
}