using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CauldronRecipeManager : MonoBehaviour
{
    private static CauldronRecipeManager _instance;
    public static CauldronRecipeManager Instance { get { return _instance; } }
    private CauldronRecipe[] _allRecipes = new CauldronRecipe[0];

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        _allRecipes = Resources.LoadAll<CauldronRecipe>("Cauldron Recipes");

    }


    public CauldronRecipe GetValidRecipe(List<IngredientItemSO> inputIngredients)
    {
        //We need to search through all possible cauldron recipes and find one whos InitialIngredients matches exactly with inputIngredients
        //Cause if the player put in too many we will get a failed recipe

        for (int i = 0; i < _allRecipes.Length; i++) 
        {
            //The input ingredients needs to match in count to the amount for a recipe to start
            //Too many inputs? then this recipe isnt valid
            //Not enough? Could run into an issue with the bottom statement where it they put 1 of the valid ingredients but not all
            if (inputIngredients.Count != _allRecipes[i].GetInitialIngredients().Length)
                continue;


            if (inputIngredients.Except(_allRecipes[i].GetInitialIngredients()).Any())
                continue;
            
            
            //If the previous statement was false, then that means that the current index of allRecipes perfectly matches with the inputIngredients
            return _allRecipes[i];
        }

        //If we went through the whole loop then there is no valid recipe and so we return a null to show a failed recipe
        return null;
    }
}
