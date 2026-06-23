using UnityEngine;

public class RitualRecipeManager : MonoBehaviour
{
    //This will be a singleton
    private static RitualRecipeManager _instance;

    public static RitualRecipeManager Instance { get { return _instance; } }


    private RitualRecipe[] _allRecipes = new RitualRecipe[0];

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        
        _allRecipes = Resources.LoadAll<RitualRecipe>("Ritual Recipes");
    
    }

    public RitualRecipe[] GetRecipeList(RitualPlatePiece centerPlate, RitualPlatePiece middlePlate, RitualPlatePiece outerPlate)
    {
        RitualRecipe[] smallerList = _allRecipes;

        if (!centerPlate && !middlePlate && !outerPlate)
            return null;

        if (centerPlate)
        {
            smallerList = System.Array.FindAll(smallerList, recipe => recipe.RequiredCenter.PieceID == centerPlate.PieceID);
        }
        if(middlePlate)
        {
            smallerList = System.Array.FindAll(smallerList, recipe => recipe.RequiredMiddle.PieceID == middlePlate.PieceID);
        }
        if(outerPlate)
        {
            smallerList = System.Array.FindAll(smallerList, recipe => recipe.RequiredOuter.PieceID == outerPlate.PieceID);
        }

        return smallerList;
    }

}
