using System;
using UnityEngine;


public interface IInfusionRecipeCondition 
{
    public bool Evaluate(IngredientInstance centralIngredient, IngredientInstance[] outerIngredients);
}
