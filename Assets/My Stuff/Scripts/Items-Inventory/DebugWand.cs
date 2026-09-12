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


    MasterSpellcaster _caster;
    XRGrabInteractable _interactable;

    private void Awake()
    {
        _caster = GetComponent<MasterSpellcaster>();
        _interactable = GetComponent<XRGrabInteractable>();

        _caster.SetSpellDefinition(_ChosenSpell);
    }

    private void OnEnable()
    {
        _interactable.activated.AddListener(Wand_ActivatePress);
        _interactable.deactivated.AddListener(Wand_ActivateRelease);

        _interactable.selectExited.AddListener(Wand_Dropped);
    }
    private void OnDisable()
    {
        _interactable.activated.RemoveListener(Wand_ActivatePress);
        _interactable.deactivated.RemoveListener(Wand_ActivateRelease);

        _interactable.selectExited.RemoveListener(Wand_Dropped);
    }

    void Wand_ActivatePress(ActivateEventArgs args)
    {
        if(_interactable.isSelected)
            _caster.SpellButton_Press(SpellButton.Primary);
    }
    void Wand_ActivateRelease(DeactivateEventArgs args)
    {
        if (_interactable.isSelected)
            _caster.SpellButton_Release(SpellButton.Primary);
    }

    void Wand_Dropped(SelectExitEventArgs args)
    {
        _caster.CancelAllCasting();
    }

    [Button]
    void ChangeSpell()
    {
        _caster.SetSpellDefinition(_NewSpell);

        _NewSpell = null;
    }
}
