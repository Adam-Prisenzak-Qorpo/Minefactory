using System.Collections.Generic;
using Minefactory.Factories.Recipes;
using Minefactory.Storage;
using UnityEngine;

namespace Minefactory.Game
{

    [CreateAssetMenu(fileName = "GameState", menuName = "Game/State")]
    public class GameState : ScriptableObject
    {
        public Interaction interaction = Interaction.None;

        public List<ItemRecipe> itemRecipes = new();

        public GameState()
        {
            var parts = new List<RecipePart>();
            parts.Add(new RecipePart("iron_raw", 3));
            var recipe = new ItemRecipe("iron", parts);
            itemRecipes.Add(recipe);
        }

    }

}