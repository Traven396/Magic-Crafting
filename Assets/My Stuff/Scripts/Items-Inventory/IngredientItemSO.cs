using Alchemy.Inspector;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient Item", menuName = "Crafting/Ingredient/Basic Ingredient")]
public class IngredientItemSO : BaseItemSO
{
    protected override ItemType _Type => ItemType.Ingredient;
    [SerializeField] private Sprite Icon;
    [SerializeField] private bool IsDiscoverable = true;
    [SerializeField] private List<AspectTag> _AspectTags;
    [SerializeField] [TextArea(4,10)] private string Description;




    [Title("Processing Methods")]
    [SerializeField] MeltableIngredientSO _MeltedIngredient;
    bool meltableIngredient => _MeltedIngredient != null;
    [ShowIf("meltableIngredient")]
    [SerializeField] int _MeltedAmount;
    public MeltableIngredientSO MeltedIngredient { get => _MeltedIngredient; }
    public int MeltedAmount { get => _MeltedAmount; }




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
