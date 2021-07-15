using Spacetaurant.Crafting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class RecipeCollection : ComponentCollection<RecipeButton>
    {
        protected override Sorter<RecipeButton>[] Sorters => throw new System.NotImplementedException();
    }
}
