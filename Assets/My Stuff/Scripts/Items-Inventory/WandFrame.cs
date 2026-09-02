using AgeOfEnlightenment.Stats;
using Kryz.CharacterStats;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class WandFrame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform WandCap;
    [SerializeField] Transform WandBottom;
    [SerializeField] public XRSocketInteractor GemSocket;

    [Header("Layouts")]
    [SerializeField] WandFrameLayout UnattachedLayout;
    //We dont need a layout for when it spawns as that will just be the default positions and such it is saved as

    [Space(10)]
    [SerializeField] List<ItemStatModifier> statModifierList;

    public List<ItemStatModifier> StatModifierList { get { return statModifierList; } }
    Renderer capRenderer, bottomRenderer;

    public Mesh TopMesh {  get { return WandCap.GetComponent<MeshFilter>().sharedMesh; } }
    public Mesh BottomMesh { get { return WandBottom.GetComponent<MeshFilter>().sharedMesh; } }

    public Material TopMaterial { get { return capRenderer.sharedMaterial; } }
    public Material BottomMaterial { get { return bottomRenderer.sharedMaterial; } }

    XRGrabInteractable grabInteractable;

    private void Start()
    {
        capRenderer = WandCap.GetComponent<Renderer>();
        bottomRenderer = WandBottom.GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(SetPositionsBasedOnInteractor);
        grabInteractable.selectExited.AddListener(InitialSpawnFirstGrabFix);

        GemSocket.selectEntered.AddListener(SpellGemAttach);
        GemSocket.selectExited.AddListener(SpellGemDetach);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(SetPositionsBasedOnInteractor);
        grabInteractable.selectExited.RemoveListener(InitialSpawnFirstGrabFix);

        GemSocket.selectEntered.RemoveListener(SpellGemAttach);
        GemSocket.selectExited.RemoveListener(SpellGemDetach);
    }
    public void InitializeFrameStats(List<ItemStatModifier> baseStatModifiers, List<ContainedLiquidMetal> liquidMetalComponents)
    {
        var highestAmount = 0f;
        Material finalMaterial = null;

        // Build a merged set of stat modifiers starting from the base list
        var cumulativeList = new Dictionary<(StatDefinitionSO, StatModType), float>();

        if (baseStatModifiers != null)
        {
            foreach (var mod in baseStatModifiers)
            {
                var key = (mod.StatDefinition, mod.ModifierType);

                if (cumulativeList.TryGetValue(key, out var cur)) 
                    cumulativeList[key] = cur + mod.Value;
                else 
                    cumulativeList[key] = mod.Value;
            }
        }

        // Apply liquid metal influences proportionally to their share of the total liquid amount
        if (liquidMetalComponents != null && liquidMetalComponents.Count > 0)
        {
            int totalAmount = liquidMetalComponents.Sum(lm => lm.amount);

            if (totalAmount > 0)
            {
                foreach (var lm in liquidMetalComponents)
                {
                    if (lm.ingredient == null) continue;

                    float proportion = (float)lm.amount / (float)totalAmount;

                    if (proportion > highestAmount)
                    {
                        highestAmount = proportion;
                        finalMaterial = lm.ingredient.AssociatedMaterial;
                    }

                    var ingredientModifier = lm.ingredient.MeltedStatModifiers;

                    if (ingredientModifier == null || ingredientModifier.Count == 0) continue;

                    foreach (var mod in ingredientModifier)
                    {
                        var key = (mod.StatDefinition, mod.ModifierType);

                        float proportionalStrengthValue = mod.Value * proportion;

                        if (cumulativeList.TryGetValue(key, out var cur)) 
                            cumulativeList[key] = cur + proportionalStrengthValue;
                        else 
                            cumulativeList[key] = proportionalStrengthValue;
                    }
                }
            }
        }

        // Create the final list and store it to the serialized field
        var result = new List<ItemStatModifier>();

        foreach (var modifier in cumulativeList)
        {
            var key = modifier.Key;

            result.Add(new ItemStatModifier(key.Item1, modifier.Value, key.Item2));
        }

        // Replace backing list with computed modifiers
        this.statModifierList = result;

        // Update the material of the wand frame based on the highest proportion liquid metal
        if(finalMaterial != null)
        {
            capRenderer.material = finalMaterial;
            bottomRenderer.material = finalMaterial;
        }
    }

    public void InitialSpawnFirstGrabFix(SelectExitEventArgs args)
    {
        //if (args.interactorObject is XRDirectInteractor || args.interactorObject is XRRayInteractor)
        //{
        //    var rb = GetComponent<Rigidbody>();

        //    rb.useGravity = true;
        //    rb.isKinematic = false;


        //    grabInteractable.selectExited.RemoveListener(InitialSpawnFirstGrabFix); 
        //}
    }


    public void SetPositionsBasedOnInteractor(SelectEnterEventArgs args)
    {
        if(args.interactorObject is XRDirectInteractor directInteractor || args.interactorObject is XRRayInteractor)
        {
            //We are being grabbed by the player's hands. We set to the unattached layout
            Unattach();
        } else if(args.interactorObject is XRSocketInteractor socketInteractor)
        {
            //We are being grabbed by a socket. We are being attached to a wand
            AttachToWand();
        }
    }

    void AttachToWand()
    {
        WandCap.localPosition = Vector3.zero;
        WandCap.localEulerAngles = Vector3.zero;
        WandBottom.localPosition = Vector3.zero;
        WandBottom.localEulerAngles = Vector3.zero;


        GemSocket.enabled = true;
    }
    void Unattach()
    {
        WandCap.localPosition = UnattachedLayout.CapPosition;
        WandCap.localEulerAngles = UnattachedLayout.CapRotation;
        WandBottom.localPosition = UnattachedLayout.BottomPosition;
        WandBottom.localEulerAngles = UnattachedLayout.BottomRotation;


        if (GemSocket.hasSelection)
        {
            //If, when we detach the frame from the wand core, we had a gem inserted we want that gem to fall off.
            //So we unparent it, and manually deselect it
            var gem = GemSocket.firstInteractableSelected;

            gem.transform.parent = null;

            GemSocket.interactionManager.UnregisterParentRelationship(gem, grabInteractable);

            GemSocket.interactionManager.SelectExit(GemSocket, gem);
        }


        GemSocket.enabled = false;
    }

    void SpellGemAttach(SelectEnterEventArgs args)
    {
        GetComponent<Rigidbody>().IgnoreCollision(args.interactableObject.transform.GetComponent<Rigidbody>());

        args.interactableObject.transform.parent = GemSocket.attachTransform;

        args.manager.RegisterParentRelationship(args.interactableObject, grabInteractable);
    }
    void SpellGemDetach(SelectExitEventArgs args)
    {
        GetComponent<Rigidbody>().IgnoreCollision(args.interactableObject.transform.GetComponent<Rigidbody>(), false);

        args.manager.UnregisterParentRelationship(args.interactableObject, grabInteractable);
    }




    [Serializable]
    struct WandFrameLayout
    {
        public Vector3 CapPosition;
        public Vector3 CapRotation;

        public Vector3 BottomPosition;
        public Vector3 BottomRotation;
    }
}
