using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cauldron Recipe", menuName = "Recipes/Cauldron Recipe")]
public class CauldronRecipe : ScriptableObject
{
    [ListViewSettings(ShowBoundCollectionSize = false)]
    [SerializeField] private IngredientItemSO[] InitialIngredients;
    [SerializeField] private CauldronStepEffects InitialStepEffects;
    [SerializeReference] public List<ICauldronRecipeStep> Steps = new();

    [Title("Recipe Outputs")]
    [SerializeField] int MaxDunks = 1;
    [ListViewSettings(Reorderable = false, ShowBoundCollectionSize = false)]
    [SerializeField] List<DunkableItemOutput> DunkConversions = new();
    [SerializeField] int MaxBottled = 10;
    //Idk how we will do actual potions for getting bottled?

    public IngredientItemSO[] GetInitialIngredients()
    {
        return InitialIngredients;
    }
    public CauldronStepEffects GetInitialEffects()
    {
        return InitialStepEffects;
    }
    public GameObject CheckDunkConversion(IngredientItemSO inputItem)
    {
        if(inputItem == null) return null;

        var validDunk = DunkConversions.Where(dunk => dunk.Input == inputItem);

        if (validDunk.Count() > 1)
            Debug.LogError(name + " has invalid conversion recipes. Same input shows up twice");

        if(validDunk.Any())
        {
            return validDunk.First().Output;
        }

        return null;
    }
    public int GetMaxDunks() { return MaxDunks; }
}
public enum StirType
{
    None,
    Clockwise,
    Counter
}
[Serializable]
public struct DunkableItemOutput
{
    public IngredientItemSO Input;
    public GameObject Output;
}
