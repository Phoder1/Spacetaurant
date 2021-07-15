using Spacetaurant.Crafting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class RecipeCollection : ButtonCollection<RecipeButton, RecipeSO>
    {
        protected override Sorter<RecipeButton>[] Sorters => throw new System.NotImplementedException();
    }
}
