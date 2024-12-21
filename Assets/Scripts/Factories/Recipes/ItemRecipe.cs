using System.Collections.Generic;
using Minefactory.Storage.Items;

namespace Minefactory.Factories.Recipes
{
    public class RecipePart
    {
        public string item;
        public int quantity;

        public RecipePart(string item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }

        public string FullName => $"{quantity}x {item}";
    }

    public enum RecipeType
    {
        Crafting,
        Smelting
    }
    public class ItemRecipe
    {
        public string outputItemName { get; set; }
        public List<RecipePart> parts = new List<RecipePart>();
        public RecipeType type { get; set; }

        public ItemRecipe(string outputItemName, List<RecipePart> parts, RecipeType type = RecipeType.Crafting)
        {
            this.outputItemName = outputItemName;
            this.parts = parts;
            this.type = type;
        }
    }
}