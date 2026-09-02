using Alchemy.Inspector;
using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InfusionAltar : MonoBehaviour
{
    [Title("Fun Temporary Stuff")]
    [SerializeField] Transform LeftHand;
    [SerializeField] Transform RightHand;
    [SerializeField] AudioClip ClapSoundClip;
    public InputActionReference ManualRecipeStarter;
    [SerializeField] bool DebugMessages = false;
    //This will have a refernece to the 7 pedestals.

    //The central main one
    //6 outer ingredients
    [Title("Scene References")]
    [SerializeField] InfusionPedestal CentralPedestal;

    [SerializeField] List<InfusionPedestal> OuterPedestals = new List<InfusionPedestal>();

    [Title("Special Effects")]
    [SerializeField] Transform FloatingCrystal;
    [SerializeField] float CrystalHeightChange = .3f;
    [SerializeField] float CrystalHeightChangeSpeed = 2f;
    [SerializeField] float CrystalRotationSpeed = 2f;

    IngredientInstance[] insertedOuterIngredients;

    InfusionRitualRecipeSO[] AllPossibleRecipes;

    InfusionRitualRecipeSO currentRecipeInProgress;

    private float clapCooldown = 0f;

    private Vector3 previousHandLocationLeft, previousHandLocationRight;
    private Vector3 leftHandVelocity, rightHandVelocity;
    bool altarPrimedForClap = false;
    Sequence resultFloatSequence;

    private void OnEnable()
    {
        CentralPedestal.IngredientSelected.AddListener(CentralIngredientInserted);
        CentralPedestal.IngredientRemoved.AddListener(CentralIngredientRemoved);

        foreach (InfusionPedestal pedestal in OuterPedestals)
        {
            pedestal.IngredientSelected.AddListener(OuterIngredientChanged);
            pedestal.IngredientRemoved.AddListener(OuterIngredientChanged);
        }
    }
    private void OnDisable()
    {
        CentralPedestal.IngredientSelected.RemoveListener(CentralIngredientInserted);
        CentralPedestal.IngredientRemoved.RemoveListener(CentralIngredientRemoved);

        foreach (InfusionPedestal pedestal in OuterPedestals)
        {
            pedestal.IngredientSelected.RemoveListener(OuterIngredientChanged);
            pedestal.IngredientRemoved.RemoveListener(OuterIngredientChanged);
        }
    }

    private void Awake()
    {
        AllPossibleRecipes = Resources.LoadAll<InfusionRitualRecipeSO>("Ritual Recipes");

        insertedOuterIngredients = new IngredientInstance[OuterPedestals.Count];

        Sequence floatingCrystalSequence = DOTween.Sequence();

        floatingCrystalSequence.Append(FloatingCrystal.DOLocalMoveY(FloatingCrystal.localPosition.y + CrystalHeightChange, CrystalHeightChangeSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo))
            .Join(FloatingCrystal.DOLocalRotate(FloatingCrystal.transform.localRotation.eulerAngles + Vector3.up * 90f, CrystalRotationSpeed).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear));
        floatingCrystalSequence.Play();
    }

    private void Update()
    {
        if (altarPrimedForClap)
        {
            //We have a cooldown for the clapping, that way it doesnt try and call the method like a million times
            if (clapCooldown <= 0)
            {
                if (currentRecipeInProgress) return;

                //We only calculate when its off cooldown to save on processing slightly.
                CalculateHandVelocity();

                if (CheckForClap() || ManualRecipeStarter.action.WasPressedThisFrame())
                {
                    clapCooldown = 1.5f;
                    SoundManagerSO.PlayClipAtPoint(ClapSoundClip, RightHand.position, .5f, 0.07f, 0.05f);
                    //We have "clapped" when the altar was ready to try and check for a recipe.
                    //We played a little sound effect for vibes
                    //We now need to check if there is an actual valid recipe to try and craft

                    CheckForValidRecipe();
                }
            }
            else
            {
                clapCooldown -= Time.deltaTime;
            }
        }
    }
    void CalculateHandVelocity()
    {
        if (previousHandLocationLeft != null)
        {
            leftHandVelocity = (LeftHand.position - previousHandLocationLeft) / Time.deltaTime;
        }
        if (previousHandLocationRight != null)
        {
            rightHandVelocity = (RightHand.position - previousHandLocationRight) / Time.deltaTime;
        }


        previousHandLocationLeft = LeftHand.position;
        previousHandLocationRight = RightHand.position;
    }
    private bool CheckForClap()
    {
        float leftHandDotXVel = Vector3.Dot(leftHandVelocity, LeftHand.right);
        float rightHandDotXVel = Vector3.Dot(rightHandVelocity, -RightHand.right);

        if (leftHandDotXVel > 0.7f && rightHandDotXVel > 0.7f && Vector3.Distance(LeftHand.position, RightHand.position) < 0.15f && leftHandVelocity.magnitude > 1.3f && rightHandVelocity.magnitude > 1.3f)
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }

    void CentralIngredientInserted()
    {
        //Check for valid recipe. And if we do have one, then we start crafting it.
        //Otherwise we play like an error sound
        altarPrimedForClap = true;
    }
    void CentralIngredientRemoved()
    {
        altarPrimedForClap = false;
    }
    void OuterIngredientChanged()
    {
        for (int i = 0; i < OuterPedestals.Count; i++)
        {
            insertedOuterIngredients[i] = OuterPedestals[i].GetInsertedIngredient();
        }
    }
    void CheckForValidRecipe()
    {
        if(CentralPedestal.GetInsertedIngredient() == null)
        {
            InvalidRecipe();
            return;
        }

        var currentPossibleRecipes = AllPossibleRecipes.Where(rec => rec.CentralIngredientRequired == CentralPedestal.GetInsertedIngredient().Item);

        if (currentPossibleRecipes.Any())
        {
            foreach (var possibleRecipe in currentPossibleRecipes)
            {
                if(possibleRecipe.CheckRecipeIngredients(CentralPedestal.GetInsertedIngredient(), insertedOuterIngredients))
                {
                    //If one recipe we checked had everything match up then we activate it as our valid recipe
                    ValidRecipe(possibleRecipe);
                    break;
                }
            }
        }
        else
        {
            InvalidRecipe();
        }
    }
    void ValidRecipe(InfusionRitualRecipeSO validRecipeToStart)
    {
        //We have found a valid recipe and now we are going to begin it.
        currentRecipeInProgress = validRecipeToStart;

        if (validRecipeToStart.RequirePattern)
        {
            //We need to enable the star grid so we can draw something
            if(DebugMessages)
                Debug.Log("We have a star recipe. It is " + validRecipeToStart.name);
        }
        else
        {
            //If the recipe doesn't require a pattern to be drawn then we are just going to craft the final item.
            if(DebugMessages)
                Debug.Log("We have a no pattern recipe. It is " + validRecipeToStart.name);

            //We might wanna play some kind of spinning fancy animation with all the ingredients before actually making the output. But for now we will just make the output.
            CreateRecipeOutput();

            CentralPedestal.ClearIngredient();

            foreach (InfusionPedestal pd in OuterPedestals)
            {
                pd.ClearIngredient();
            }

            CentralIngredientRemoved();

            currentRecipeInProgress = null;
        }
    }
    void InvalidRecipe()
    {
        //Something was wrong with the recipe setup. We will play like an error sound, and reset the altar in some way
        if(DebugMessages)
            Debug.Log("We didn't have a recipe, lets figure out why." + "\n" +
                "Central Ingredient: " + CentralPedestal.GetInsertedIngredient().gameObject.name + "\n" +
                "Outer Ingredients: " + insertedOuterIngredients[0].gameObject.name + "\n" +
                insertedOuterIngredients[1].gameObject.name + "\n" +
                insertedOuterIngredients[2].gameObject.name + "\n" +
                insertedOuterIngredients[3].gameObject.name + "\n" +
                insertedOuterIngredients[4].gameObject.name + "\n" +
                insertedOuterIngredients[5].gameObject.name);
    }

    void CreateRecipeOutput()
    {
        //We have had a valid recipe, and are going to create the output of it. We will spawn the item, and then remove all the ingredients from the pedestals.
        var recipeOutput = currentRecipeInProgress.CreateRecipeOutput(CentralPedestal.GetInsertedIngredient().gameObject);

        MakeResultFloat(recipeOutput);

        
    }

    void MakeResultFloat(GameObject result)
    {
        resultFloatSequence?.Kill(); // Kill any existing sequence to avoid overlapping animations

        resultFloatSequence = DOTween.Sequence();
        resultFloatSequence.Append(result.transform.DOMoveY(result.transform.position.y + 0.05f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo))
            .Join(result.transform.DORotate(result.transform.rotation.eulerAngles + Vector3.up * 20f, 2f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear));

        resultFloatSequence.Play();

        result.GetComponent<XRGrabInteractable>().selectEntered.AddListener(KillFloatTween);
    }

    void KillFloatTween(SelectEnterEventArgs args)
    {
        if (resultFloatSequence != null)
        {
            resultFloatSequence.Kill();
        }

        args.interactableObject.selectEntered.RemoveListener(KillFloatTween);
    }
}

