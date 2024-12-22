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

            itemRecipes.Add(new ItemRecipe("gold", new List<RecipePart>{
                new("gold_raw", 2)
            }, RecipeType.Smelting));

            itemRecipes.Add(new ItemRecipe("void", new List<RecipePart>{
                new("void_raw", 2)
            }, RecipeType.Smelting));

            itemRecipes.Add(new ItemRecipe("belt", new List<RecipePart>{
                new("iron", 2)
            }));

            itemRecipes.Add(new ItemRecipe("furnace", new List<RecipePart>{
                new("stone", 5),
                new("silicone", 2)
            }));

            itemRecipes.Add(new ItemRecipe("crafter", new List<RecipePart>{
                new("iron", 5),
                new("stone", 2),
                new("silicone", 2)
            }));

            itemRecipes.Add(new ItemRecipe("auto_miner", new List<RecipePart>{
                new("iron", 5),
                new("silicone", 2),
                new("void", 2)
            }));

            itemRecipes.Add(new ItemRecipe("void_storage", new List<RecipePart>{
                new("iron", 3),
                new("silicone", 3),
                new("void", 3)
            }));

            itemRecipes.Add(new ItemRecipe("colony", new List<RecipePart>{
                new("iron", 30),
                new("silicone", 5),
                new("gold", 15),
                new("stone", 10)
            }));
        }

    }

}