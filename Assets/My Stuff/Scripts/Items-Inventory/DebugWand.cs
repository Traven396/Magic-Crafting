using AgeOfEnlightenment.Spellcasting;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DebugWand : MonoBehaviour
{
    [SerializeField] SpellDefinitionSO _NewSpell;
    [Space(10f)]
    [SerializeField] SpellDefinitionSO _ChosenSpell;
    [SerializeField] Transform _CastOrigin;


    MasterSpellcaster _caster;
    XRGrabInteractable _interactable;

    private void Awake()
    {
        _caster = GetComponent<MasterSpellcaster>();
        _interactable = GetComponent<XRGrabInteractable>();

        _caster.SetSpellDefinition(_ChosenSpell);
        _caster.SetCastOrigin(_CastOrigin);
    }

    private void OnEnable()
    {
        _interactable.activated.AddListener(Wand_ActivatePress);
        _interactable.deactivated.AddListener(Wand_ActivateRelease);
    }
    private void OnDisable()
    {
        _interactable.activated.RemoveListener(Wand_ActivatePress);
        _interactable.deactivated.RemoveListener(Wand_ActivateRelease);
    }

    void Wand_ActivatePress(ActivateEventArgs args)
    {
        _caster.SpellButton_Press(SpellButton.Primary);
    }
    void Wand_ActivateRelease(DeactivateEventArgs args)
    {
        _caster.SpellButton_Release(SpellButton.Primary);
    }


    [Button]
    void ChangeSpell()
    {
        _caster.SetSpellDefinition(_NewSpell);

        _NewSpell = null;
    }
}
