using Alchemy.Inspector;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ritual Recipe", menuName = "Crafting/Recipes/Infusion Ritual Recipe")]
public class InfusionRitualRecipeSO : ScriptableObject
{
    [Title("Required Ingredients")]
    [SerializeField] IngredientItemSO _CentralIngredientRequired;
    public IngredientItemSO CentralIngredientRequired { get { return _CentralIngredientRequired; } }
    [SerializeField] IngredientItemSO[] _OuterIngredientsRequired;
    public IngredientItemSO[] OuterIngredientsRequired { get { return _OuterIngredientsRequired; } }

    [Title("Additional Conditions")]
    [SerializeReference]
    IInfusionRecipeCondition[] _AdditionalConditions;
    public IInfusionRecipeCondition[] AdditionalConditions { get { return _AdditionalConditions; } }

    [Title("Possible Outputs")]
    [SerializeField] protected bool _RequirePattern;
    public bool RequirePattern { get { return _RequirePattern; } }

    [ShowIf("_RequirePattern")]
    [SerializeField]
    protected StarPatternOutput[] _PatternOutputs;

    [HideIf("_RequirePattern")]
    [SerializeField]
    protected GameObject _RecipeOutput;
    



    public bool CheckRecipeIngredients(IngredientInstance inputCentralIngredient, IngredientInstance[] inputOuterIngredients)
    {
        var cleanedOuterIngredientList = inputOuterIngredients.Where(x => x != null).ToArray();

        //We input either too many or too little ingredients
        if (cleanedOuterIngredientList.Length != _OuterIngredientsRequired.Length) return false;

        if (inputCentralIngredient.Item != CentralIngredientRequired) return false;

        if (_AdditionalConditions.Length > 0)
        {
            foreach (var condition in _AdditionalConditions)
            {
                if (!condition.Evaluate(inputCentralIngredient, cleanedOuterIngredientList))
                {
                    Debug.Log("Condition failed");
                    return false;
                }
            }

        }

        if (_OuterIngredientsRequired.Length == 0) return true;

        //If we sort both arrays by the name of the ingredient we can check and see if they are the same by comparing each element.
        Array.Sort(_OuterIngredientsRequired, new IngredientInstanceComparer());
        Array.Sort(cleanedOuterIngredientList, new IngredientInstanceComparer());

        if (_OuterIngredientsRequired.SequenceEqual(Array.ConvertAll(cleanedOuterIngredientList, ing => ing.Item))) {
            return true;
        }
        else {  
            return false; 
        }
    }

    public GameObject GetPatternOutput(StarPattern drawnPattern)
    {
        if (!_RequirePattern) return null;

        if(_PatternOutputs.Count() == 0) return null;

        var validOutput = _PatternOutputs.Where(pattern => pattern.InputConstellation == drawnPattern);

        if (validOutput.Count() == 0 || validOutput.Count() > 1) return null;

        return validOutput.First().OutputItem;
    }

    public virtual GameObject CreateRecipeOutput(GameObject oldCentralIngredient)
    { 
        GameObject spawnedCraftingOutput = Instantiate(_RecipeOutput, oldCentralIngredient.transform.position, Quaternion.identity);

        return spawnedCraftingOutput;
    }


    [Serializable]
    public struct StarPatternOutput
    {
        public StarPattern InputConstellation;
        public GameObject OutputItem;
    }

    class IngredientInstanceComparer : IComparer<IngredientInstance>, IComparer<IngredientItemSO>
    {
        public int Compare(IngredientInstance x, IngredientInstance y)
        {
            return String.Compare(x.Item.name, y.Item.name, StringComparison.OrdinalIgnoreCase);
        }

        public int Compare(IngredientItemSO x, IngredientItemSO y)
        {
            return String.Compare(x.name, y.name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
