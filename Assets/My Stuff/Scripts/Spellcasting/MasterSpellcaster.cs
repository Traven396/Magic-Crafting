namespace AgeOfEnlightenment.Spellcasting
{
    using Alchemy.Inspector;
    using UnityEngine;

    public class MasterSpellcaster : MonoBehaviour
    {
        [Title("Spellcasting Settings")]
        [SerializeField] SpellDefinitionSO _ActiveSpell;
        [Title("Scene References")]
        [SerializeField] Transform _CastOriginPoint;
        [SerializeField] Transform _CasterTransform;

        //Need a dictionary to hold all the active spell sessions, and when creating a new one we pass along a unique key for them to check back with

        //This is the method called every update
        public void Spell_Tick()
        {

        }


        public void Spell_PrimaryInput_Press()
        {

        }

        public void Spell_PrimaryInput_Hold()
        {

        }

        public void Spell_PrimaryInput_Release()
        {

        }


        public void Spell_SecondaryInput_Press()
        {

        }

        public void Spell_SecondaryInput_Hold()
        {

        }

        public void Spell_SecondaryInput_Release()
        {

        }
    }

}