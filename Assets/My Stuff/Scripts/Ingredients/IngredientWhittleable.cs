using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(IngredientInstance))]
public class IngredientWhittleable : IngredientKnifeProcessable
{
    public Image DebugImage;
    private float currentProcessAmount = 0;
    private int currentStep = 0;

    IngredientInstance selfIngredient;
    private XRInteractionManager interactionManager;

    private void Awake()
    {
        selfIngredient = GetComponent<IngredientInstance>();
        interactionManager = FindFirstObjectByType<XRInteractionManager>();

        if(!interactionManager)
            Debug.LogError("No XRInteractionManager found in the scene, IngredientWhittleable will not work without one");
    }

    [Button]
    private void CompleteStep()
    {
        currentStep++;
        OnStepComplete?.Invoke();
        
        if(currentStep >= RequiredProcessSteps)
        {
            OnProcessComplete?.Invoke();
            Debug.Log("Completed all " + RequiredProcessSteps + " steps and finished the process.");
            FinishProcess();
        }
        else
        {
            Debug.Log("Completed step " + currentStep + " of " + RequiredProcessSteps);
        }
    }

    public override void Process(float progress)
    {
        currentProcessAmount += progress;

        DebugImage.fillAmount = currentProcessAmount / RequiredProcessAmount;

        if (currentProcessAmount >= RequiredProcessAmount)
        {
            currentProcessAmount = 0;
            currentStep++;

            OnStepComplete?.Invoke();
            if (currentStep == RequiredProcessSteps)
            {
                OnProcessComplete?.Invoke();

                Debug.Log("Completed the whole processing");

                FinishProcess();
            }
        }
    }


    private void FinishProcess()
    {
        //We need to disable this object
        //Unattach it from the hand
        //Spawn the output item for each amount of result
        //Have the interactor holding this grab the first new one
        //Destroy this object
        
        var currentGrabber = selfIngredient.GetInteractable().GetOldestInteractorSelecting();

        if(currentGrabber != null)
            interactionManager.SelectExit(currentGrabber, selfIngredient.GetInteractable());

        for (int i = 0; i < OutputAmount; i++)
        {
            GameObject newOutput = Instantiate(ProcessOutput, transform.position, transform.rotation);

            if (currentGrabber != null)
            {
                if (i == 0)
                {
                    //Have the hand grab the first one
                    if (newOutput.TryGetComponent(out XRGrabInteractable newGrabInteractable))
                    {
                        interactionManager.SelectEnter(currentGrabber, newGrabInteractable);
                    }
                    else
                    {
                        Debug.LogError("Process output does not have an IXRSelectInteractable component, cannot grab it");
                    }
                } 
            }
        }

        
        Destroy(gameObject);
    }
}
