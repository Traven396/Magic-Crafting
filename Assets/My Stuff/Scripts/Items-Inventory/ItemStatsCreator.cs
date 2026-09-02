using AgeOfEnlightenment.Stats;
using Alchemy.Inspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Stats Creator", menuName = "Managers/Item Stats Creator")]
public class ItemStatsCreator : ScriptableObject
{
    private static ItemStatsCreator instance;
    public static ItemStatsCreator Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<ItemStatsCreator>("Item Stats Creator");
            return instance;
        }
    }

    [SerializeField] WandStats BaseWandStats;


    public WandStats CreateWandStats(ProtoWand baseWand)
    {
        WandStats modifiedStats = BaseWandStats;

        WandFrame frame = baseWand._AttachedWandFrame;

        foreach (ItemStatModifier modifier in frame.StatModifierList)
        {
            if (modifier.TryApplyModifier(modifiedStats.Durability))
                continue;
            if (modifier.TryApplyModifier(modifiedStats.SpellEfficiency))
                continue;
            if (modifier.TryApplyModifier(modifiedStats.SpellPower))
                continue;
            if (modifier.TryApplyModifier(modifiedStats.MaxMana))
                continue;
            //This surely is not being done in the correct way. I dont care though
        }

        return modifiedStats;
    }
}
[Serializable]
public struct WandStats
{
    //CurrentSpell
    public MagicElement Element;
    public Stat Durability;
    public Stat SpellEfficiency;
    public Stat SpellPower;
    public Stat MaxMana;
}