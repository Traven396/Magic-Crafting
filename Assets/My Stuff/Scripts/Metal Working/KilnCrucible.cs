using Alchemy.Inspector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KilnCrucible : MonoBehaviour
{
    //This class will just be a container to hold all of the ingredients placed into it.
    //Have it work like in Hydroneer, where once its inside the crucible it cant collide with any of the other things inside of it, to save on space.
    //They should still be grabbable so you can get them out afterwards if needed.
    [SerializeField] int CoolSpeed = 1;
    [SerializeField] TemperatureController temperatureController;
    public int CurrentHeat => temperatureController.TemperatureInt;
    [SerializeField] int MaxContainedLiquid = 250;
    [SerializeField] Transform ColliderParent;
    [SerializeField] Transform PourOrigin;
    [SerializeField] GameObject StreamPrefab;

    List<IngredientInstance> currentInsertedMeltables = new();
    [SerializeField] List<ContainedLiquidMetal> currentContainedLiquidMetal = new();

    Collider[] childColliders;
    SkinnedMeshRenderer selfRenderer;

    LiquidStream currentStream;

    float lastMeltCheckTemperature;


    //Pouring stuff
    [Title("Pouring Settings")]
    [SerializeField] bool DebugPouring = false;
    bool isPouring;

    [Button]
    void MeltCurrentIngredients()
    {
        if(currentInsertedMeltables.Count > 0)
        {
            MeltInternals(true);
        }
    }


    private void Awake()
    {
        childColliders = ColliderParent.GetComponentsInChildren<Collider>();
        selfRenderer = GetComponent<SkinnedMeshRenderer>();

        if (temperatureController == null)
            temperatureController = GetComponent<TemperatureController>();

        // Match the old FixedUpdate cooling rate of CoolSpeed per physics tick.
        temperatureController.SetPassiveCoolRate(CoolSpeed / Time.fixedDeltaTime);
        lastMeltCheckTemperature = temperatureController.Temperature;
        temperatureController.TemperatureChanged += OnTemperatureChanged;
    }

    void OnDestroy()
    {
        if (temperatureController == null)
            return;

        temperatureController.TemperatureChanged -= OnTemperatureChanged;
    }

    void OnTemperatureChanged(float newTemperature)
    {
        if (newTemperature <= lastMeltCheckTemperature)
        {
            lastMeltCheckTemperature = newTemperature;
            return;
        }

        lastMeltCheckTemperature = newTemperature;
        MeltInternals(false);
    }

    private void Update()
    {
        //We are only trying to pour if there is any liquid inside of the crucible. Otherwise it would be wasted processing
        if (currentContainedLiquidMetal.Count > 0)
            AttemptPour();
    }

    #region Pouring
    void AttemptPour()
    {
        //We need to check the angle that the crucible is currently at
        //Baseline angle at 100% full is 40 degrees, and at 0% full is 100.
        //The angle we are checking against will be based on the amount of liquid in the crucible as well.
        //Angles of rotation would be checked on every axis except for Y axis

        //If we are currently pouring liquid is determined by calculating the angle
        //Saving it in a temporary value so we can detect when it changes
        bool pourCheck = CalculatePour();

        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;

            //We were not previously pouring, and now we have started
            if (isPouring)
            {
                StartPouring();
            }
            else
            {
                EndPouring();
            }
        }

        if (isPouring)
        {
            ActivelyPour();
        }

    }

    bool CalculatePour()
    {
        //Calculate what the current tipping angle of the crucible is based on its rotation, and return that value as a float
        float currentAngle = Vector3.Angle(transform.up, Vector3.up);

        float currentFillPercent = (float)currentContainedLiquidMetal.Sum(lm => lm.amount) / (float)MaxContainedLiquid;

        float requiredPourAngle = Mathf.Lerp(80, 40f, currentFillPercent);

        //Debug.Log("Current Angle: " + currentAngle + " Required Angle: " + requiredPourAngle + " Current Fill Percent: " + currentFillPercent);

        if (DebugPouring)
            requiredPourAngle = 40f;

        return currentAngle >= requiredPourAngle;
    }

    void StartPouring()
    {
        currentStream = CreateStream();

        currentStream.Begin();
    }

    void ActivelyPour()
    {
        if (Physics.Raycast(PourOrigin.transform.position, Vector3.down, out RaycastHit hit, 2f, Physics.AllLayers, QueryTriggerInteraction.Collide))
        {

            if (hit.collider.TryGetComponent(out LiquidInput input))
            {
                PourIntoObject(input);
            }
            else
                PourIntoObject(null);

        }
        else
        {
            PourIntoObject(null);
        }

        UpdateVisualLiquid();

        if (currentContainedLiquidMetal.Count == 0)
        {
            //If all of the liquid has been poured then we stop the visuals
            isPouring = false;
            EndPouring();
        }
    }

    void PourIntoObject(LiquidInput input)
    {
        //We need to calculate how much liquid we are pouring out of the crucible based on the angle and the amount of liquid inside of it
        int totalPourAmount = currentContainedLiquidMetal.Sum(lm => lm.amount);

        //We will pour out .1% of the total amount per second, so we multiply that by Time.deltaTime to get the amount for this frame
        int pourAmountThisFrame = Mathf.CeilToInt(totalPourAmount * 0.001f * Time.deltaTime);

        //We will only pour out as much as we have, so we clamp it between 0 and the total amount
        pourAmountThisFrame = Mathf.Clamp(pourAmountThisFrame, 0, totalPourAmount);

        //We will pour out the first liquid in the list, and remove it from the list if it is empty
        ContainedLiquidMetal liquidToPour = currentContainedLiquidMetal[0];

        if (input)
        {
            input.TryAddLiquid(liquidToPour.ingredient, pourAmountThisFrame);
        }

        liquidToPour.amount -= pourAmountThisFrame;

        if (liquidToPour.amount <= 0)
        {
            currentContainedLiquidMetal.RemoveAt(0);
        }
        else
        {
            currentContainedLiquidMetal[0] = liquidToPour;
        }
    }

    void EndPouring()
    {
        if(currentStream)
            currentStream.End();

        currentStream = null;
    }

    LiquidStream CreateStream()
    {
        GameObject streamObject = Instantiate(StreamPrefab, PourOrigin.position, Quaternion.identity, transform);
        return streamObject.GetComponent<LiquidStream>();
    } 

    void UpdateVisualLiquid()
    {
        if (currentContainedLiquidMetal.Count < 1)
        {
            selfRenderer.SetBlendShapeWeight(0, 100f);
        } 
        else
            selfRenderer.SetBlendShapeWeight(0, Mathf.Clamp((1 - ((float)currentContainedLiquidMetal.Sum(lm => lm.amount) / (float)MaxContainedLiquid)) * 100f - 15, 0f, 100f));

        
    }
    #endregion

    #region Heating + Melting
    void MeltInternals(bool ignoreHeat)
    {
        List<IngredientInstance> onesToRemove = new();
        currentInsertedMeltables.ForEach(m =>
        {

            if (m.Item.CanMelt && (m.Item.MeltingTemperature <= CurrentHeat || ignoreHeat))
            {
                int currentFill = currentContainedLiquidMetal.Sum(liquid => liquid.amount);

                ContainedLiquidMetal addedMetal = new ContainedLiquidMetal();
                addedMetal.ingredient = m.Item;
                addedMetal.amount = Mathf.Clamp(m.LiquidYield, 0, MaxContainedLiquid - currentFill);

                if (addedMetal.amount != 0)
                {
                    var sameLiquid = currentContainedLiquidMetal.Where(lm => lm.ingredient == m.Item).ToList();

                    if (sameLiquid.Any())
                    {
                        addedMetal.amount += sameLiquid[0].amount;

                        currentContainedLiquidMetal[currentContainedLiquidMetal.IndexOf(sameLiquid[0])] = addedMetal;

                        //Debug.Log("Added to an already existing metal named: " + addedMetal.ingredient.name + " for a total of: " + addedMetal.amount);
                    }
                    else
                    {
                        //Debug.Log("We added a new metal named: " + addedMetal.ingredient.name + " at an amount of: " + addedMetal.amount);
                        currentContainedLiquidMetal.Add(addedMetal);
                    }
                }
                else
                {
                    //Debug.Log("we melted an ingredient but the crucible was already full so it's contents were wasted. Uh oh");
                }

                onesToRemove.Add(m);
                Destroy(m.gameObject);

            }
        });

        currentInsertedMeltables = currentInsertedMeltables.Except(onesToRemove).ToList();

        UpdateVisualLiquid();
    }



    #endregion


    #region Ingredient Attaching
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IngredientInstance instance))
        {
            if (!currentInsertedMeltables.Contains(instance))
            {
            if (instance.Item.CanMelt)
                {
                    AttachIngredient(instance, other);
                    instance.GetInteractable().selectEntered.AddListener(IngredientGrabbedOut);
                } 
            }
        }
    }

    void AttachIngredient(IngredientInstance instance, Collider instanceCollider)
    {
        instance.AttachedRB.isKinematic = true;
        instance.transform.parent = transform;

        foreach (Collider col in childColliders)
        {
            Physics.IgnoreCollision(col, instanceCollider, true);
        }

        currentInsertedMeltables.Add(instance);
    }


    void IngredientGrabbedOut(SelectEnterEventArgs args)
    {
        IngredientInstance instance = (args.interactableObject as XRBaseInteractable).GetComponent<IngredientInstance>();
        Collider firstCol = args.interactableObject.colliders[0];

        DetachIngredient(instance, firstCol);

        args.interactableObject.selectEntered.RemoveListener(IngredientGrabbedOut);
        args.interactableObject.selectExited.AddListener(DumbSelectExitFix);
    }

    void DumbSelectExitFix(SelectExitEventArgs args)
    {
        var dumbassRB = (args.interactableObject as XRBaseInteractable).GetComponent<Rigidbody>();

        dumbassRB.isKinematic = false;
        dumbassRB.useGravity = true;

        args.interactableObject.selectExited.RemoveListener(DumbSelectExitFix);
    }

    void DetachIngredient(IngredientInstance instance, Collider instanceCollider)
    {
        foreach (Collider col in childColliders)
        {
            Physics.IgnoreCollision(col, instanceCollider, false);
        }

        currentInsertedMeltables.Remove(instance);
    }


    #endregion
}
[Serializable]
public struct ContainedLiquidMetal
{
    public IngredientItemSO ingredient;
    public int amount;
}
