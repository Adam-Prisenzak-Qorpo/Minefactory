using System.Collections.Generic;
using Minefactory.Factories.Recipes;
using Minefactory.Player;
using Minefactory.Storage;
using UnityEngine;

namespace Minefactory.Game
{

    [CreateAssetMenu(fileName = "GameState", menuName = "Game/State")]
    public class GameState : ScriptableObject
    {
        public Interaction interaction = Interaction.None;
        public PlayerController player;

        public List<ItemRecipe> itemRecipes = new();

        public GameState()
        {
            itemRecipes.Add(new ItemRecipe("iron", new List<RecipePart>{
                new ("iron_raw", 3)
            }, RecipeType.Smelting));

            itemRecipes.Add(new ItemRecipe("belt", new List<RecipePart>{
                new("iron", 2)
            }));
        }

    }

}