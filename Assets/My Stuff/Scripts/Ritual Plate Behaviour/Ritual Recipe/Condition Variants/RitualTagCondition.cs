using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RitualTagCondition : IRecipeCondition
{
    
    public List<RitualTagConditionAspect> Aspects = new List<RitualTagConditionAspect>();

    
    public bool Evaluate(CurrentRitualInfo ritualInfo)
    {
        IngredientInstance[] currentIngredients = ritualInfo.ingredients;
        RitualTagConditionAspect[] uniqueTags = Aspects.Where(x => x.Comparison == ConditionType.ContainsUnique).ToArray();

        foreach (var aspect in Aspects) 
        { 
            if(!aspect.CheckIngredients(currentIngredients.ToList(), out IngredientInstance _))
            {
                return false;
            }
        } 
        
        //foreach (var ingredient in currentIngredients.Where(ing => ing.Tags.HasFlag(uniqueTags[0].WantedTag)))
        //{
        //    var removedIngredientList = currentIngredients.Where(x => x != ingredient).ToList();
        //    foreach (var aspect in uniqueTags.Skip(1))
        //    {
        //        if (aspect.CheckIngredients(removedIngredientList, out Ingredient nextIngredient))
        //        {
        //            if(aspect == uniqueTags.Last())
        //            {
        //                return true;
        //            }

        //            removedIngredientList.Remove(nextIngredient);
        //        }
        //    }
        //}

        //Sort the requirement list by ones that have the most potential options, meaning that less backtracking has to happen
        uniqueTags = uniqueTags.OrderBy(r => currentIngredients.Count(i => i.Item.ContainsAspect(r.WantedTag))).ToArray();

        return Solve(uniqueTags, currentIngredients, 0, new HashSet<IngredientInstance>());
        //Search to find an ingredient that satisfies the first aspect
        //Remove that ingredient from the list
        //Search through the next aspect with the remaining ingredients, if it fails, go back to the first aspect and try the next valid ingredient, and so on until either all aspects are satisfied or all options are exhausted
    }

    private bool Solve(RitualTagConditionAspect[] requiredTags, IngredientInstance[] ingredientList, int reqIndex, HashSet<IngredientInstance> usedItems)
    {
        // all requirements satisfied
        if (reqIndex >= requiredTags.Length)
            return true;

        AspectSO neededTag = requiredTags[reqIndex].WantedTag;

        foreach (IngredientInstance ingredient in ingredientList)
        {
            // already consumed
            if (usedItems.Contains(ingredient))
                continue;

            // doesn't satisfy this requirement
            if (ingredient.Tags.Where(tag => tag.Aspect == neededTag).Count() < 1)
                continue;

            // try using it
            usedItems.Add(ingredient);

            if (Solve(requiredTags, ingredientList, reqIndex + 1, usedItems))
                return true;

            // backtrack
            usedItems.Remove(ingredient);
        }

        // no valid assignment for this requirement
        return false;
    }


}
[Serializable]
public class RitualTagConditionAspect
{
    public AspectSO WantedTag;
    public ConditionType Comparison;
    public int Amount;
    

    public bool CheckIngredients(List<IngredientInstance> ingredientsToCheck, out IngredientInstance chosenIngredient)
    {
        
        List<IngredientInstance> validIngredients = ingredientsToCheck.Where(x => x.Item.ContainsAspect(WantedTag)).ToList();
        int validIngredientCount = validIngredients.Count;


        //Idk if this works how its supposed to yet.
        //CoPilot made it and it looks ok but im not certain
        int validAspectTotal = validIngredients.Sum(x => x.Tags.Where(t => t.Aspect == WantedTag).Sum(t => t.Strength));

        if (validIngredientCount == 0 && Comparison != ConditionType.LessThan && Comparison != ConditionType.LessThanOrEqual)
        {
            chosenIngredient = null;
            return false;
        }

        chosenIngredient = validIngredients[0];

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

            case ConditionType.ContainsUnique:
                //We return true here cause at this point we know we have at least 1 valid ingredient since we check above for count 0
                return true;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}