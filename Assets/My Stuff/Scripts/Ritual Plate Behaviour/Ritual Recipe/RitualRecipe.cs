using Alchemy.Inspector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ritual Recipe", menuName = "Crafting/Recipes/Ritual Recipe")]
public class RitualRecipe : ScriptableObject
{
    [Title("Crafting Result")]
    [SerializeField]
    public RitualRecipeOutput[] PossiblePatterns;
    [Title("Crafting Requirements")]
    public IngredientItemSO CentralIngredient;
    public int RequiredCenterID;
    public int RequiredMiddleID;
    public int RequiredOuterID;

    [SerializeReference]
    public List<IRecipeCondition> AdditionalConditions = new List<IRecipeCondition>();
    

    public bool ValidRecipeConfiguration(CurrentRitualInfo ritualInfo)
    {
        if (ritualInfo.centerPiece == null || ritualInfo.middlePiece == null || ritualInfo.outerPiece == null)
        {
            Debug.Log("All plates not placed");
            return false; 
        }
        if (ritualInfo.centerPiece.PieceID != RequiredCenterID ||
            ritualInfo.middlePiece.PieceID != RequiredMiddleID ||
            ritualInfo.outerPiece.PieceID != RequiredOuterID)
        { 
            Debug.Log("Incorrect plate pieces");
            return false; 
        }
        if (ritualInfo.centralIngredient.Item != CentralIngredient)
        { 
            Debug.Log("Incorrect center ingredient");
            return false; 
        }
        foreach (var condition in AdditionalConditions)
        {
            if (!condition.Evaluate(ritualInfo))
            {
                Debug.Log("Condition failed");
                return false; 
            }
        }
        return true;
    }


    [Serializable]
    public struct RitualRecipeOutput
    {
        public StarPattern InputConstellation;
        public GameObject OutputItem;
    }
}
