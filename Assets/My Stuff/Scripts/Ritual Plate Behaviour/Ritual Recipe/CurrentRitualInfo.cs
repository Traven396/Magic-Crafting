using UnityEngine;

public class CurrentRitualInfo
{
    public RitualPlatePiece centerPiece;
    public RitualPlatePiece middlePiece;
    public RitualPlatePiece outerPiece;

    public IngredientInstance centralIngredient;
    public IngredientInstance[] ingredients;


    public CurrentRitualInfo(IngredientInstance centralIngredient, IngredientInstance[] ingredients, RitualPlatePiece outerPiece, RitualPlatePiece middlePiece, RitualPlatePiece centerPiece)
    {
        this.centralIngredient = centralIngredient;
        this.ingredients = ingredients;

        this.outerPiece = outerPiece;
        this.middlePiece = middlePiece;
        this.centerPiece = centerPiece;
    }
}

