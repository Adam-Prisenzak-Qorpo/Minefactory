using Factories.Recipes;
using Minefactory.Factories.Recipes;
using Minefactory.Game;
using UnityEngine;

namespace Minefactory.Factories
{
    public class CrafterBehaviour : MonoBehaviour
    {
        public GameObject itemListContainer;
        public GameObject materialListContainer;
        public SelectRecipeEvent selectRecipeEvent;

        private ItemRecipe recipe;
        // Start is called before the first frame update
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
            }
        }

        public void OnSelectRecipe(ItemRecipe newRecipe)
        {
            Debug.Log("SelectRecipe event");
            SelectRecipe(newRecipe);
            selectRecipeEvent?.Invoke(recipe);
        }
        public void SelectRecipe(ItemRecipe newRecipe)
        {
            recipe = newRecipe;
            var script = materialListContainer.GetComponent<MaterialListBehaviour>();
            script.SetRecipe(recipe);
        }

    }
}