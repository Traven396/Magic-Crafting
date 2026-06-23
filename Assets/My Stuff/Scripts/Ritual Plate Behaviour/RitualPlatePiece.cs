using UnityEngine;
using UnityEngine.Windows;

public class RitualPlatePiece : MonoBehaviour
{
    public enum PlateLocation
    {
        Center,
        Middle,
        Outer
    }

    [SerializeField] private PlateLocation location;
    public int PieceID;

    [SerializeField] private RitualIngredientInput[] Inputs;

    private void Awake()
    {
        switch (location)
        {
            case PlateLocation.Center:
                gameObject.tag = "Center";
                break;
            case PlateLocation.Middle:
                gameObject.tag = "Middle";
                break;
            case PlateLocation.Outer:
                gameObject.tag = "Outer";
                break;
         }
    }

    public bool TryGetIngredient(out IngredientInstance[] ingredientList)
    {
        ingredientList = new IngredientInstance[Inputs.Length];
        for (int i = 0; i < Inputs.Length; i++)
        {
            if (Inputs[i].GetValidIngredient(out IngredientInstance ingredientPart))
            {
                ingredientList[i] = ingredientPart;
            }
            else
            {
                ingredientList = null;
                return false;
            }
        }
        return true;
    }

}
