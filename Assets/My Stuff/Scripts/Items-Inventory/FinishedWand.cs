using AgeOfEnlightenment.Spellcasting;
using AgeOfEnlightenment.Stats;
using Alchemy.Inspector;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FinishedWand : MonoBehaviour
{
    //This will be the full class responsible for controlling the wand after it has been made.
    //Motion controls
    //Grab interaction
    //Casting of spells
    [Title("Spellcasting Settings")]
    [SerializeField] SpellDefinitionSO _GemSpell;
    [SerializeField] WandStats _Stats;
    [SerializeField] Transform _WandTip;

    [Title("Object References")]
    [SerializeField] Transform _WandCore;
    MeshRenderer _CoreRenderer;
    MeshFilter _CoreFilter;

    [Space(12f)]

    [SerializeField] Transform _WandFrameTop;
    MeshRenderer _TopFrameRenderer;
    MeshFilter _TopFrameFilter;
    [SerializeField] Transform _WandFrameBottom;
    MeshRenderer _BottomFrameRenderer;
    MeshFilter _BottomFrameFilter;

    [Space(12f)]

    [SerializeField] Transform _SpellGem;
    MeshRenderer _GemRenderer;
    MeshFilter _GemFilter;

    XRGrabInteractable _GrabInteractable;


    MasterSpellcaster _Spellcaster;

    private void Awake()
    {
        _Spellcaster = GetComponent<MasterSpellcaster>();

        if (!_Spellcaster)
            Debug.LogError($"{name} does not have a spellcaster in it. Fix that");

        _GrabInteractable = GetComponent<XRGrabInteractable>();

        _CoreRenderer = _WandCore.GetComponent<MeshRenderer>();
        _CoreFilter = _WandCore.GetComponent<MeshFilter>();

        _TopFrameRenderer = _WandFrameTop.GetComponent<MeshRenderer>();
        _TopFrameFilter = _WandFrameTop.GetComponent<MeshFilter>();

        _BottomFrameRenderer = _WandFrameBottom.GetComponent<MeshRenderer>();
        _BottomFrameFilter = _WandFrameBottom.GetComponent<MeshFilter>();

        _GemRenderer = _SpellGem.GetComponent<MeshRenderer>();
        _GemFilter = _SpellGem.GetComponent<MeshFilter>();
    }

    private void OnEnable()
    {
        _GrabInteractable.activated.AddListener(ActivateWand_Press);
        _GrabInteractable.deactivated.AddListener(ActivateWand_Release);
    }
    private void OnDisable()
    {
        _GrabInteractable.activated.RemoveListener(ActivateWand_Press);
        _GrabInteractable.deactivated.RemoveListener(ActivateWand_Release);
    }
    //It will also intialize itself to determine it's full stats when it is created.
    public void InitializeWand(ProtoWand initialWand)
    {
        //For right now we are going to be assuming that the wand core is the basic wood

        _CoreFilter.mesh = initialWand.CoreMesh;
        _CoreRenderer.material = initialWand.CoreMaterial;

        
        _TopFrameFilter.mesh = initialWand._AttachedWandFrame.TopMesh;
        _TopFrameRenderer.material = initialWand._AttachedWandFrame.TopMaterial;

        _BottomFrameFilter.mesh = initialWand._AttachedWandFrame.BottomMesh;
        _BottomFrameRenderer.material = initialWand._AttachedWandFrame.BottomMaterial;

        _GemFilter.mesh = initialWand._AttachedSpellGem.Filter.sharedMesh;
        _GemRenderer.material = initialWand._AttachedSpellGem.Renderer.sharedMaterial;






        _Stats = ItemStatsCreator.Instance.CreateWandStats(initialWand);

        var GemList = Resources.LoadAll<SpellGemConfiguration>("Spell Gem Config");

        var chosenSpell = GemList.Where(gem => gem.Cut == initialWand._AttachedSpellGem.CutType && gem.Element == initialWand._AttachedSpellGem.Element);

        if (chosenSpell.Count() == 0)
        {
            Debug.LogError("The chosen spell gem doesn't have an associated spell. Element: " + initialWand._AttachedSpellGem.Element.ToString() + " Cut: " + initialWand._AttachedSpellGem.CutType);
            return;
        }

        _GemSpell = chosenSpell.First().Spell;

        //We also need to set up the visuals for this item. Like making sure the correct frame and material are applied to it.

        _Spellcaster.SetSpellDefinition(_GemSpell);
        _Spellcaster.SetCastOrigin(_WandTip);
    }

    void ActivateWand_Press(ActivateEventArgs args)
    {
        _Spellcaster.SpellButton_Press(SpellButton.Primary);
    }
    void ActivateWand_Release(DeactivateEventArgs args)
    {
        _Spellcaster.SpellButton_Release(SpellButton.Primary);
    }
}
