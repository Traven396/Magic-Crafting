using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class CauldronRecipeIngredientStep : ICauldronRecipeStep
{
    [ListViewSettings(Reorderable = false, ShowBoundCollectionSize = false)]
    [SerializeField] private List<IngredientItemSO> RequiredIngredients;
    [SerializeField] private StirType RequiredStir;
    [SerializeField] private CauldronStepEffects CompletedStepEffects;

    public bool ExecuteStep(CauldronRecipeContext context)
    {
        //This is an XOR comparison. Meaning that if these dont match up, we get a false
            //So if the player DID stir, and WASNT supposed to (true ^ false)
            //If they DIDNT stir, and WERE supposed to (false ^ true)
        //Both of the above statements would return a true, and mean the player was wrong
        if (RequiredStir != context.ProvidedStir)
        {
                return false;
        }

        return CheckIngredients(context);
    }

    private bool CheckIngredients(CauldronRecipeContext context)
    {
        //Did we input the correct amount of ingredients? Simple way to check before running the more expensive check
        if(context.AddedIngredients.Length != RequiredIngredients.Count)
        {
            return false;
        }

        if (RequiredIngredients.Except(Array.ConvertAll(context.AddedIngredients, item => item.Item)).Any())
        {
            //We check if there is anything in the Requirements list that does not appear in the AddedIngredients list
            //If we have any left over then they didnt match
            return false;
        }

        return true;
    }

    public CauldronStepEffects GetEffects()
    {
        return CompletedStepEffects;
    }
}
