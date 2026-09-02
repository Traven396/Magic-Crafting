using System;
using UnityEngine;
[Serializable]
public class InfusionCondition_WandComplete : IInfusionRecipeCondition
{
    public bool Evaluate(IngredientInstance centralIngredient, IngredientInstance[] outerIngredients)
    {
        ProtoWand centralWand = centralIngredient.GetComponent<ProtoWand>();

        if (!centralWand)
            return false;

        if (!centralWand.ValidWand)
            return false;

        return true;
    }
}
