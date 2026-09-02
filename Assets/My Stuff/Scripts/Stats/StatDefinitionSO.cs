using Kryz.CharacterStats;
using System;
using UnityEngine;

namespace AgeOfEnlightenment.Stats
{
    [CreateAssetMenu(fileName = "New Stat Definition", menuName = "Stats/Stat Definition")]
    public class StatDefinitionSO : ScriptableObject
    {
        //Not sure what code would need to be contained in here? Potentially like a maximum that the stat could be? Otherwise this is just to be used as a unique identifier
        [TextArea(3, 10)]
        [SerializeField] private string description;
    }

    //Some kind of struct to tie the SO definition of a stat and the actual changeable value from the Kryzarel one
    [Serializable]
    public class Stat
    {
        public StatDefinitionSO AssociatedStat;
        public CharacterStat StatValue;
    }

    [Serializable]
    public class ItemStatModifier
    {
        [SerializeField] private StatDefinitionSO statDefinition;
        [SerializeField] private float value;
        [SerializeField] private StatModType modifierType;
        public StatDefinitionSO StatDefinition => statDefinition;
        public float Value => value;
        public StatModType ModifierType => modifierType;
        public ItemStatModifier(StatDefinitionSO statDefinition, float value, StatModType modifierType)
        {
            this.statDefinition = statDefinition;
            this.value = value;
            this.modifierType = modifierType;
        }

        public bool IsCombinable(ItemStatModifier modifier)
        {
            if(modifier.ModifierType != modifierType) return false;

            if(modifier.StatDefinition != statDefinition) return false;

            return true;
        }

        public void Combine(ItemStatModifier modifier) 
        {
            value += modifier.value;
        }
        public bool TryApplyModifier(Stat stat)
        {
            if(stat.AssociatedStat != statDefinition) return false;

            stat.StatValue.AddModifier(new StatModifier(Value, ModifierType));

            return true;
        }
    }
}