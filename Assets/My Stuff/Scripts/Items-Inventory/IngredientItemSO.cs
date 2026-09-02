using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;
using AgeOfEnlightenment.Stats;

[CreateAssetMenu(fileName = "New Ingredient Item", menuName = "Crafting/Ingredient/Basic Ingredient")]
public class IngredientItemSO : BaseItemSO
{
    protected override ItemType _Type => ItemType.Ingredient;
    [SerializeField] private Sprite Icon;
    [SerializeField] private bool IsDiscoverable = true;
    [SerializeField] private List<AspectTag> _AspectTags;
    [SerializeField] private Material _AssociatedMaterial;
    [SerializeField] [TextArea(4,10)] private string Description;




    [Title("Metal Working")]
    [SerializeField] private bool _CanMelt;
    [ShowIf("CanMelt")]
    [SerializeField] private int _MeltingTemperature;
    [ShowIf("CanMelt")]
    [SerializeField] private Color _LiquidColor;
    [ShowIf("CanMelt")]
    [SerializeField] private List<ItemStatModifier> _MeltedStatModifiers;
    /// <summary>
    /// Whether this material can be melted in a crucible. Individual prefabs define
    /// their yield through IngredientInstance.LiquidYield.
    /// </summary>
    public bool CanMelt => _CanMelt;
    public int MeltingTemperature => _MeltingTemperature;
    public Color LiquidColor => _LiquidColor;
    public List<ItemStatModifier> MeltedStatModifiers => _MeltedStatModifiers;

    public Material AssociatedMaterial => _AssociatedMaterial;

    [SerializeField] bool _IsAlchemyIngredient;
    public bool AlchemyIngredient { get => _IsAlchemyIngredient; }

    public List<AspectTag> GetTags()
    {
        return _AspectTags;
    }

    public Sprite GetIcon()
    {
        return Icon;
    }

    public string GetDescription()
    {
        return Description;
    }

    public bool Discoverable()
    {
        return IsDiscoverable;
    }

    public bool ContainsAspect(AspectSO aspect)
    {
        foreach (AspectTag tag in _AspectTags)
        {
            if (tag.Aspect == aspect)
            {
                return true;
            }
        }
        return false;
    }
}
[System.Flags]
public enum IngredientTags
{
    None = 0,
    Fire = 1,
    Water = 2,
    Earth = 4,
    Air = 8,
    Light = 16,
    Dark = 32,
    Organic = 64,
    Inorganic = 128,
    Plant = 256,
    Beast = 512,
    Life = 1024,
    Death = 2048
}
