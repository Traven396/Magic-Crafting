namespace AgeOfEnlightenment.Spellcasting
{
    using Alchemy.Inspector;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Spells/Base Spell", fileName = "New Spell")]
    public class SpellDefinitionSO : ScriptableObject
    {
        //The spell definition is just a place to hold and store the settings for a whole spell.
        //For multiple casting methods you add more activation routes, which contain all of the settings needed to cutomize the spell.

        //Later on this object will contain the information of the spell like descriptions, tooltips, icons, maybe a color and spell circle?
        [SerializeField] List<SpellActivationRoute> _ActivationRoutes;

        public IReadOnlyList<SpellActivationRoute> ActivationRoutes => _ActivationRoutes;

    }

}