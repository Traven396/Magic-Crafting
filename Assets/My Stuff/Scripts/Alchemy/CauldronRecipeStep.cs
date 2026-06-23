using UnityEngine;

public interface ICauldronRecipeStep
{
    public bool ExecuteStep(CauldronRecipeContext context);
    public CauldronStepEffects GetEffects();
}

public class CauldronRecipeContext
{
    public IngredientInstance[] AddedIngredients;
    public StirType ProvidedStir;

    public CauldronRecipeContext(IngredientInstance[] addedIngredients, StirType stirring)
    {
        AddedIngredients = addedIngredients;
        ProvidedStir = stirring;
    }
}
