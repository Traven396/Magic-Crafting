using UnityEngine;

[CreateAssetMenu(fileName = "New Melted Ingredient", menuName = "Crafting/Ingredient/Melted Ingredient")]
public class MeltableIngredientSO : ScriptableObject
{
    [SerializeField] int meltingTemperature = 0;
    [SerializeField] Color liquidColor;

    public Color LiquidColor { get => liquidColor; }
    public int MeltingTemperature { get => meltingTemperature; }
}
