using UnityEngine;

public class WandInfusionRecipeSO : InfusionRitualRecipeSO
{
    public override GameObject CreateRecipeOutput(GameObject oldCentralIngredient)
    {
        GameObject spawnedCraftingOutput = Instantiate(_RecipeOutput, oldCentralIngredient.transform.position, Quaternion.identity);

        spawnedCraftingOutput.GetComponent<FinishedWand>().InitializeWand(oldCentralIngredient.GetComponent<ProtoWand>());

        return spawnedCraftingOutput;
    }
}
