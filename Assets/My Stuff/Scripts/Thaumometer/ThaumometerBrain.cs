using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ThaumometerBrain : MonoBehaviour
{
    public TMP_Text debugText;

    public XRSocketInteractor IngredientSocket;
    public GameObject AspectPrefab;
    public HandControlArea LeftHandArea;
    public HandControlArea RightHandArea;
    [Title("Aspect Search Settings")]
    public float aspectDiscoverThreshold = 0.5f;
    public float aspectDiscoverCooldown = 3f;

    private IngredientInstance _currentIngredient;
    private AspectGhost[] currentAspectPreviews = new AspectGhost[0];

    private bool ValidHandConfiguration => (LeftHandArea.ValidController && RightHandArea.ValidController);
    private float leftHandValue => LeftHandArea.GetControllerValue();
    private float rightHandValue => RightHandArea.GetControllerValue();

    private int maxAspectCount => currentAspectPreviews.Length;
    private int discoveredAspectCount = 0;

    private bool ingredientAlreadyDiscovered = false;

    private float aspectDiscoverTimer = 0f;

    //Divide the incoming value, which ranges from -180 to 180, by 6 to get a value between -30 and 30, then round that to the nearest tenth and clamp it to a max of 16, since the aspect locations will be within a 16 unit radius from the center
    private Vector2 handValueVector => new(Mathf.Clamp(Mathf.Round((leftHandValue / 6) * 10f) * .1f, -16, 16), Mathf.Clamp(Mathf.Round((rightHandValue / 6) * 10f) * 0.1f, -16, 16));

    [Title("Events")]
    public UnityEvent<IngredientInstance> OnIngredientFullyScanned;

    public UnityEvent<IngredientInstance> OnIngredientSelected;
    public UnityEvent<IngredientInstance> OnIngredientDeselected;
    public UnityEvent<IngredientInstance, int> OnSingleAspectSolve;


    [Button]
    public void Solve_Aspect_Step()
    {
        if (_currentIngredient != null && !DiscoveriesSingleton.Instance.IsIngredientDiscovered(_currentIngredient))
        {
            OnSingleAspectSolve?.Invoke(_currentIngredient, discoveredAspectCount);
            Debug.Log("Solved aspect " + (discoveredAspectCount + 1) + " of " + maxAspectCount);
            discoveredAspectCount++;

            if(discoveredAspectCount >= maxAspectCount)
            {
                DiscoverIngredient();
                Debug.Log("Fully discovered ingredient: " + _currentIngredient.Item.GetName());
            }
        }
    }

    private void Update()
    {
        if(IngredientSocket.hasSelection)
            if(!ingredientAlreadyDiscovered)
                CalculateHandSearch();


        debugText.text = "Hand Vector: " + handValueVector + "\n" +
                         "Current Aspect Location: " + (currentAspectPreviews.Length > 0 ? currentAspectPreviews[discoveredAspectCount].HiddenLocation.ToString() : "N/A") + "\n" +
                         "Valid Hand Config: " + ValidHandConfiguration;
    }

    public void IngredientSelect()
    {
        _currentIngredient = IngredientSocket.GetOldestInteractableSelected().transform.GetComponent<IngredientInstance>();

        discoveredAspectCount = 0;

        OnIngredientSelected?.Invoke(_currentIngredient);

        if (DiscoveriesSingleton.Instance.IsIngredientDiscovered(_currentIngredient))
        {
            ingredientAlreadyDiscovered = true;
            //This is where we would want to display the already discovered aspects in some way
            debugText.text = "Ingredient already discovered: " + _currentIngredient.Item.GetName();
        }
        else
        {
            CreateAspectPreviews();

            ingredientAlreadyDiscovered = false;
        }
    }
    public void IngredientDeselect()
    {
        _currentIngredient = null;

        ingredientAlreadyDiscovered = false;

        ClearAspectPreviews();

        OnIngredientDeselected?.Invoke(_currentIngredient);
    }

    private void CreateAspectPreviews()
    {
        int iCount = _currentIngredient.Tags.Count;

        currentAspectPreviews = new AspectGhost[iCount];

        for (int i = 0; i < iCount; i++) {

            currentAspectPreviews[i] = Instantiate(AspectPrefab, IngredientSocket.transform.position + new Vector3(-0.1f, 0.1f, 0), Quaternion.identity * Quaternion.Euler(0, 90, 0)).GetComponent<AspectGhost>();
            currentAspectPreviews[i].gameObject.SetActive(false);
            currentAspectPreviews[i].transform.parent = transform;

            if (i == 0)
            {
                currentAspectPreviews[i].RandomizeLocation(null);
            }
            else
            {
                currentAspectPreviews[i].RandomizeLocation(currentAspectPreviews[i - 1].HiddenLocation);
            }
        }
    }

    private void ClearAspectPreviews()
    {
        foreach (AspectGhost aspect in currentAspectPreviews)
        {
            Destroy(aspect.gameObject);
        }
        currentAspectPreviews = new AspectGhost[0];

        discoveredAspectCount = 0;
    }

    private void CalculateHandSearch()
    {
        if (ValidHandConfiguration)
        {
            ActivateCurrentPreview();

            float distanceToCurrentAspect = Vector2.Distance(handValueVector, currentAspectPreviews[discoveredAspectCount].HiddenLocation);
            float smoothedDistanceToAspect = Mathf.Max(distanceToCurrentAspect - aspectDiscoverThreshold, 0);

            currentAspectPreviews[discoveredAspectCount].SetOffset(smoothedDistanceToAspect);

            if (smoothedDistanceToAspect == 0)
            {
                aspectDiscoverTimer += Time.deltaTime;
                if (aspectDiscoverTimer >= aspectDiscoverCooldown)
                {
                    OnSingleAspectSolve?.Invoke(_currentIngredient, discoveredAspectCount);

                    aspectDiscoverTimer = 0f;
                    discoveredAspectCount++;
                    if (discoveredAspectCount >= maxAspectCount)
                    {
                        DiscoverIngredient();

                        //Here is where we would communicate to the manager or whatever that this ingredient has been fully scanned and can be added to the list
                        //I still need to come up with a proper system for the Item management, since over time they will need to be put into inventories too.
                        //Current idea is a base ItemScriptableObject and have other variations inherit from it, like IngredientSO, WeaponSO, etc
                        //Tho the issue I would need to be aware of is if the spawned instances of the items will ever get unique data to them
                        //
                        //The base item will be a SO that contains all the immutable data needed, like ID and such. and then create a seperate Class that contains a reference to
                        //the base item and all the new custom data. The manager will then keep track of the new class instead of the base item, and we can just reference the base item when we need to access the immutable data. 

                        //Probably use some event to tell whatever manager which ingredient was just solved, pass along the item container or whatever so that ID can be passed over into the discovered category


                    }
                }
            }
            else
            {
                aspectDiscoverTimer = 0f;
            }
        }
        else
        {
            foreach (AspectGhost aspect in currentAspectPreviews)
                aspect.gameObject.SetActive(false);
        }
    }

    private void ActivateCurrentPreview()
    {
        for (int i = 0; i < currentAspectPreviews.Length; i++)
        {
            if (i == discoveredAspectCount)
                currentAspectPreviews[i].gameObject.SetActive(true);
            else
                currentAspectPreviews[i].gameObject.SetActive(false);
        }
    }


    private void DiscoverIngredient()
    {
        discoveredAspectCount = 0;

        ingredientAlreadyDiscovered = true;

        OnIngredientFullyScanned?.Invoke(_currentIngredient);
        ClearAspectPreviews();
    }
}
