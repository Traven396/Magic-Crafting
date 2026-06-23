using Alchemy.Inspector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CauldronBrain : MonoBehaviour, ITriggerable
{
    //Step Timer is how long the cauldron waits before performing a check for the next step
    //ResetWaterTimer is how long it takes holding your hands to the plates to reset teh water
    XRInteractionManager _manager;


    [Title("Settings")]
    [SerializeField] private float StepTimer = 7;
    [SerializeField] public float ResetWaterTimer = 2;
    [SerializeField] float ItemConversionTimer = 4;


    //Two references to the different Reset Plates on the cauldron
    [Title("References")]
    [SerializeField] CauldronResetPlate LeftPlate;
    [SerializeField] CauldronResetPlate RightPlate;
    [SerializeField] BoxCollider CauldronInternalCollider;

    //Reference to the stirring stick placed into the cauldron
    CauldronStirStick _stirringStick;

    //A boolean that is only true once the player places a hand on each reset plate
    bool resetWaterValidHands => LeftPlate.handValid && RightPlate.handValid;


    //A property for a timer used to reset the water to a default state.
    //Its a property because CauldronResetPlate uses this to fill out the circles for the hand
    public float refreshTimer { get; private set; } = 0;
    //The actual timer that is incremented to progress to the next recipe step check
    float recipeStepTimer = 0;
    //A boolean to tell the script whether to be currently counting down to the next step of a recipe. Only active when a valid recipe is about to be tried
    bool timerStarted = false;
    //If a recipe fails this tells the script to basically stop trying anything until the cauldron is reset
    bool dirtyWater = false;
    //A boolean set to true when we have completed a recipe
    bool recipeComplete = false;

    //This number ignores the first initial step, and is only for incrementing through the List Steps
    int currentRecipeStep = 0;

    //Keep track of how big at the start the trigger collider is. That way if we scale it anymore it will still work
    float initialColliderSize;
    
    //All ingredients that have been placed inside the cauldron so far
    private List<IngredientInstance> currentAddedIngredients = new();
    //A property used to determine which way the player is stirring the liquid in the cauldron
    //Range of -1 to 1
    // -1 = Counter-Clockwise
    // 1 = Clockwise
    public float CurrentStirAmount { get; private set; } = 0;

    //The current recipe the cauldron is progressing through. When the first ingredient check is done this is set to the first valid recipe returned using those ingredients
    private CauldronRecipe currentRecipe;


    Dictionary<IngredientInstance, float> convertingIngredientTimers = new();
    int convertedIngredientCount = 0;


    //Various events for other scripts to hook onto.
    [Title("Events")]
    public UnityEvent<CauldronStepEffects> CauldronStepProgress;
    public UnityEvent CauldronRecipeSuccess;
    public UnityEvent CauldronRecipeFail;
    public UnityEvent CauldronRefreshWater;
    public UnityEvent<float> CauldronStir;
    public UnityEvent CauldronBeginFoam;
    public UnityEvent CauldronEndFoam;

    public UnityEvent<IngredientInstance> CauldronIngredientConsume;
    public UnityEvent<IngredientInstance, float> CauldronIngredientConvert;

    private void Awake()
    {
        _manager = FindFirstObjectByType<XRInteractionManager>();

        if(!_manager)
        {
            Debug.Log("No interaction manager found. CauldronBrain will not work");
        }

        initialColliderSize = CauldronInternalCollider.size.y;
    }
    private void Update()
    {
        if(recipeComplete)
            CheckConversionTimers();


        if (resetWaterValidHands)
        {
            refreshTimer += Time.deltaTime;

            if(refreshTimer >= ResetWaterTimer)
            {
                refreshTimer = 0;
                RefreshWater();
            }
        }
        else
        {
            refreshTimer = 0;
        }


        if (timerStarted)
        {
            if (recipeStepTimer < StepTimer)
            {
                recipeStepTimer += Time.deltaTime;
            }
            else
            {
                //The timer has completed so we call to go to the next step

                //If we are already in the middle of a recipe then we check that, otherwise look for a valid recipe
                if (currentRecipe)
                    CauldronStep();
                else
                    CauldronFirstStep();

                ClearStep();

                recipeStepTimer = 0;
            } 
        }
    }

    void CheckConversionTimers()
    {
        var finishedTimers = convertingIngredientTimers.Where(timer => Time.time - timer.Value >= ItemConversionTimer).ToArray();
        
        foreach (var timer in finishedTimers)
        {
            convertingIngredientTimers.Remove(timer.Key);

            IngredientConversionFinish(timer.Key);
        }
    }

    void IngredientConversionFinish(IngredientInstance finishedIngredient)
    {
        if (finishedIngredient == null) 
        {
            Debug.Log("Passed ingredient conversion was null");
            return; 
        }


        //We need to check if something is already holding the interactable. If it is we need to make sure it starts holding the new ingredient
        //Otherwise we can just do the create and swap method

        GameObject convertedIngredient = Instantiate(currentRecipe.CheckDunkConversion(finishedIngredient), finishedIngredient.transform.position, finishedIngredient.transform.rotation);

        if (finishedIngredient.GetInteractable().isSelected)
        {
            var currentGrabber = finishedIngredient.GetInteractable().GetOldestInteractorSelecting();
            //Make the interactor drop the item
            _manager.SelectExit(currentGrabber, finishedIngredient.GetInteractable());

            var newGrabbable = convertedIngredient.GetComponent<XRGrabInteractable>();

            _manager.SelectEnter(currentGrabber, newGrabbable);
        }

        convertedIngredientCount++;

        //At completely full this should be 0, at completely empty it is 1.
        //Because the blend shape was made incorrectly technically
        float fillAmount = convertedIngredientCount / (float)currentRecipe.GetMaxDunks();


        //Need to move the collider in the center down
        //Both the trigger and the physical one
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (fillAmount == 1)
        {
            CauldronInternalCollider.size = new Vector3(CauldronInternalCollider.size.x, initialColliderSize * (1 - fillAmount), CauldronInternalCollider.size.z);
            CauldronInternalCollider.center = new Vector3(CauldronInternalCollider.center.x, -CauldronInternalCollider.bounds.extents.y, CauldronInternalCollider.center.z);

            CauldronIngredientConvert?.Invoke(finishedIngredient, fillAmount * 100); 
        }

        //Regardless of which path we destroy that ingredient as it becomes converted
        //But we do it after the event call so it has the location and stuff to spawn things
        Destroy(finishedIngredient.gameObject);

        if(convertingIngredientTimers.Count == 0)
        {
            CauldronEndFoam?.Invoke();
        }
    }
    private void CauldronFirstStep()
    {
        //Check to see if there is a valid recipe starting with those ingredients
        //No recipe will require stirring at the start
        currentRecipe = CauldronRecipeManager.Instance.GetValidRecipe(currentAddedIngredients.ConvertAll(item => (IngredientItemSO)item));

        //Start the steps again at 0
        currentRecipeStep = 0;
        recipeComplete = false;

        if (!currentRecipe)
        {
            //If we didnt find a recipe then we just fail the water and consume the ingredients
            RecipeFail();
            Debug.Log("No recipe found");
        }
        else
        {
            Debug.Log("Found the recipe of " + currentRecipe.name);
            CauldronStepProgress?.Invoke(currentRecipe.GetInitialEffects());

            if (currentRecipe.Steps.Count == 0)
                RecipeSuccess();
        }
    }


    private void CauldronStep()
    {
        StirType currentStir = StirType.None;

        if (CurrentStirAmount >= 0.5)
            currentStir = StirType.Clockwise;
        else if (CurrentStirAmount <= -0.5)
            currentStir = StirType.Counter;



        CauldronRecipeContext curContext = new(currentAddedIngredients.ToArray(), currentStir);


        if (currentRecipe.Steps[currentRecipeStep].ExecuteStep(curContext))
        {
            //The recipe check was succesful
            CauldronStepProgress?.Invoke(currentRecipe.Steps[currentRecipeStep].GetEffects());


            currentRecipeStep++;

            if(currentRecipeStep == currentRecipe.Steps.Count)
            {
                RecipeSuccess();
            }
        }
        else
        {

            RecipeFail();
        }

        
    }
    private void ClearStep()
    {
        currentAddedIngredients.ForEach(item => {
            CauldronIngredientConsume?.Invoke(item);
            Destroy(item.gameObject); 
        });
        
        currentAddedIngredients = new();

        CurrentStirAmount = 0;
        CauldronStir?.Invoke(CurrentStirAmount);
    }
    private void RecipeFail()
    {
        if(currentRecipe)
            Debug.Log(currentRecipe.name + " failed");

        dirtyWater = true;

        currentRecipe = null;
        currentRecipeStep = 0;

        recipeStepTimer = 0;
        timerStarted = false;

        CurrentStirAmount = 0;

        CauldronStir?.Invoke(CurrentStirAmount);
        CauldronRecipeFail?.Invoke();
    }

    private void RecipeSuccess()
    {

        Debug.Log(currentRecipe.name + " completed successfully");

        //We need the recipe to remain on succcess so that we can get the output of it later on
        //currentRecipe = null;

        recipeStepTimer = 0;
        timerStarted = false;

        recipeComplete = true;

        CurrentStirAmount = 0;

        CauldronStir?.Invoke(CurrentStirAmount);
        CauldronRecipeSuccess?.Invoke();
    }
    [Button]
    private void RefreshWater()
    {
        dirtyWater = false;
        recipeComplete = false;
        
        currentRecipe = null;
        currentRecipeStep = 0;

        refreshTimer = 0;
        recipeStepTimer = 0;
        
        if(currentAddedIngredients.Count > 0)
            timerStarted = true;

        convertingIngredientTimers.Clear();

        CauldronInternalCollider.size = new Vector3(CauldronInternalCollider.size.x, initialColliderSize, CauldronInternalCollider.size.z);
        CauldronInternalCollider.center = Vector3.zero;

        CurrentStirAmount = 0;
        //-----------------------------------------------------------------------------------------------------//
        //The ingredients will still be there and the recipe will not start again
        CauldronStir?.Invoke(CurrentStirAmount);
        CauldronRefreshWater?.Invoke();
    }
    void DunkIngredientEnter()
    {
        CauldronBeginFoam?.Invoke();
    }

    public void OnTriggerEnterCall(Collider other)
    {
        //Ingredient added or the stir stick placed into the cauldron
        
        var addedIngredient = other.gameObject.GetComponentInParent<IngredientInstance>();

        if (addedIngredient)
        {
            if (recipeComplete)
            {
                if (currentRecipe.GetMaxDunks() - convertedIngredientCount > convertingIngredientTimers.Count)
                {
                    if (currentRecipe.CheckDunkConversion(addedIngredient.Item))
                    {
                        convertingIngredientTimers.Add(addedIngredient, Time.time);

                        //The first time an convertable ingredient is added, we start the foam thing a little bit later so that it has time to fully get submerged
                        if(convertingIngredientTimers.Count == 1)
                            Invoke(nameof(DunkIngredientEnter), 0.3f);
                    } 
                }

                //We return regardless as to not do unnecessary checks when in a finished recipe state
                return;
            }



            currentAddedIngredients.Add(addedIngredient);


            if(!dirtyWater && !recipeComplete)
                timerStarted = true;
        } 
        

        if(other.TryGetComponent(out CauldronStirStick stick))
        {
            stick.startCircling = true;
            if(!_stirringStick)
                _stirringStick = stick;
        }
    }

    public void OnTriggerStayCall(Collider other)
    {
        //This will be for stirring
        if(_stirringStick)
        {
            CurrentStirAmount = Mathf.Clamp(_stirringStick.CircleAmount, -1, 1);
            CauldronStir?.Invoke(CurrentStirAmount);

            //While stirring we are setting the value to 0 so the recipe doesnt try and progress in the middle of stirring.
            recipeStepTimer = 0;
        }
    }

    public void OnTriggerExitCall(Collider other)
    {
       
        //Ingredient taken out
        var removedIngredient = other.gameObject.GetComponentInParent<IngredientInstance>();

        if (recipeComplete)
        {
            if (convertingIngredientTimers.ContainsKey(removedIngredient))
            {
                convertingIngredientTimers.Remove(removedIngredient);

                //If that was the last ingredient being converted we cancel any hanging invoke so it doesnt visually break
                //Then we call the method to cancel any foam
                if (convertingIngredientTimers.Count == 0)
                {
                    CancelInvoke(nameof(DunkIngredientEnter));
                    CauldronEndFoam?.Invoke();
                }
            }

            //Same as on trigger enter, we dont need to worry about the rest of this stuff if the recipe is completed
            return;
        }

        //Recipe still in progress
        if (currentAddedIngredients.Contains(removedIngredient))
        {
            currentAddedIngredients.Remove(removedIngredient);

            //If we remove the last ingredient and we arent in the middle of a recipe we dont need to keep the timer going
            if (currentAddedIngredients.Count == 0 && !currentRecipe)
            {
                timerStarted = false;
            }
        }

        

        if (other.TryGetComponent(out CauldronStirStick stick))
        {
            stick.startCircling = false;
            if (_stirringStick)
                _stirringStick = null;
        }
    }
}
