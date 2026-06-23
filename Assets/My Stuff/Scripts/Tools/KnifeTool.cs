using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class KnifeTool : Tool
{
    [SerializeField] private float WhittleProcessModifier = 1;
    [SerializeField] private float WhittleAngleThreshold = 0.8f;
    [SerializeField] private float WhittleHapticStrength = 0.5f;
    [SerializeField] private Transform BladeTransform;
    //Will need some kind of collider here. To detect the blade


    bool isWhittling = false;
    bool isCutting = false;

    private Rigidbody rb;
    private HapticImpulsePlayer currentHandHaptic;

    private IngredientKnifeProcessable currentCollidedIngredient;
    private Vector3 lastStepKnifePosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void UseTool()
    {
        
    }

    public void OnPickup(SelectEnterEventArgs args)
    {
        if(args.interactorObject.transform.TryGetComponent(out HapticImpulsePlayer hand))
        {
            currentHandHaptic = hand;
        }
        else
        {
            Debug.Log(args.interactorObject.transform.name + " does not have a HapticImpulsePlayer component, cannot send haptics to this hand");
        }
    }

    public void OnDrop(SelectExitEventArgs args)
    {
        currentHandHaptic = null;
    }

    public void ForceDropTest()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        XRInteractionManager manager = FindFirstObjectByType<XRInteractionManager>();

        if (grab is IXRSelectInteractable selectable)
        {
            manager.CancelInteractableSelection(selectable);
        }
        else
        {
            Debug.LogWarning("Interactable does not implement IXRSelectInteractable, cannot cancel selection.");
        }
    }


    public void KnifeEnter(IngredientKnifeProcessable ingredient)
    {
        //The forward part of the blade should be pointing towards the length wise of the ingredient. In most cases this will be the up vector
        currentCollidedIngredient = ingredient;
        lastStepKnifePosition = BladeTransform.position;
    }

    //This method is called every FixedUpdate
    public void KnifeStay()
    {
        float whittleBladeAngleStrength = Mathf.Abs(Vector3.Dot(BladeTransform.forward, currentCollidedIngredient.transform.up));
        float motionBladeAngleStrength = Vector3.Dot(rb.linearVelocity.normalized, BladeTransform.forward);


        if(whittleBladeAngleStrength > WhittleAngleThreshold && motionBladeAngleStrength > WhittleAngleThreshold)
        {
            float whittleAmount = Vector3.Distance(lastStepKnifePosition, BladeTransform.position) * WhittleProcessModifier;

            currentCollidedIngredient.Process(whittleAmount);

            float hapticStrength = Mathf.Clamp01(WhittleHapticStrength * rb.linearVelocity.magnitude);

            if(currentHandHaptic)
                currentHandHaptic.SendHapticImpulse(hapticStrength, 0.1f);
        }
        //Motion in the direction of the blade matching with the up vector of the ingredient



        lastStepKnifePosition = BladeTransform.position;
    }

    public void KnifeExit()
    {
        isWhittling = false;
        currentCollidedIngredient = null;
    }
    //What is whittling?
    //Blade direction stays mostly parallel to the length of wood
    //Keep the blade close to the wood the entire time
    //Drag it along the edge of it
    //Move at least the majority of the legnth of the wood
}
