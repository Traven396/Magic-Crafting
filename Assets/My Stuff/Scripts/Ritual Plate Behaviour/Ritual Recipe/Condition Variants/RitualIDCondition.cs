using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RitualIDCondition : IRecipeCondition
{
    public IngredientItemSO WantedID;
    public ConditionType Comparison;
    public int Amount;
    public bool Evaluate(CurrentRitualInfo ritualInfo)
    {
        List<IngredientInstance> validIngredients = ritualInfo.ingredients.Where(x => x.Item == WantedID).ToList();
        int validIngredientCount = validIngredients.Count;

        switch (Comparison)
        {
            case ConditionType.GreaterThan:
                return validIngredientCount > Amount;

            case ConditionType.GreaterThanOrEqual:
                return validIngredientCount >= Amount;

            case ConditionType.Equal:
                return validIngredientCount == Amount;

            case ConditionType.LessThan:
                return validIngredientCount < Amount;

            case ConditionType.LessThanOrEqual:
                return validIngredientCount <= Amount;


                //This default covers the NeedUnique version since this type of condition doesnt care about uniqueness
            default:
                return validIngredientCount > 0;
        }
    }
}
