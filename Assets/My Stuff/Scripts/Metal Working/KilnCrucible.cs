using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KilnCrucible : MonoBehaviour
{
    //This class will just be a container to hold all of the ingredients placed into it.
    //Have it work like in Hydroneer, where once its inside the crucible it cant collide with any of the other things inside of it, to save on space.
    //They should still be grabbable so you can get them out afterwards if needed.
    [SerializeField] int CoolSpeed = 1;
    [SerializeField] int _CurrentHeat = 0;
    public int CurrentHeat { get { return _CurrentHeat; } }
    [SerializeField] int MaxContainedLiquid = 250;
    [SerializeField] Transform ColliderParent;

    bool isHeating;

    List<IngredientInstance> currentInsertedMeltables = new();
    [SerializeField] List<ContainedLiquidMetal> currentContainedLiquidMetal = new();

    Collider[] childColliders;
    SkinnedMeshRenderer selfRenderer;

    private void Awake()
    {
        childColliders = ColliderParent.GetComponentsInChildren<Collider>();
        selfRenderer = GetComponent<SkinnedMeshRenderer>();
        
    }

    private void FixedUpdate()
    {
        if (!isHeating)
        {
            if(_CurrentHeat > 0)
                DecreaseHeat();
        }
    }

    public void BeginCooling()
    {
        isHeating = false;
    }

    public void IncreaseHeat(int heatAmount)
    {
        _CurrentHeat += heatAmount;
        isHeating = true;

        List<IngredientInstance> onesToRemove = new();
        currentInsertedMeltables.ForEach(m =>
        {
            if (m.Item.MeltedIngredient.MeltingTemperature <= _CurrentHeat)
            {
                int currentFill = currentContainedLiquidMetal.Sum(liquid => liquid.amount);

                ContainedLiquidMetal addedMetal = new ContainedLiquidMetal();
                addedMetal.meltableIngredient = m.Item.MeltedIngredient;
                addedMetal.amount = Mathf.Clamp(m.Item.MeltedAmount, 0, MaxContainedLiquid - currentFill);

                if (addedMetal.amount != 0)
                {
                    var sameLiquid = currentContainedLiquidMetal.Where(lm => lm.meltableIngredient == m.Item.MeltedIngredient).ToList();

                    if (sameLiquid.Any())
                    {
                        addedMetal.amount += sameLiquid[0].amount;

                        currentContainedLiquidMetal[currentContainedLiquidMetal.IndexOf(sameLiquid[0])] = addedMetal;

                        Debug.Log("Added to an already existing metal named: " + addedMetal.meltableIngredient.name + " for a total of: " + addedMetal.amount);
                    }
                    else
                    {
                        Debug.Log("We added a new metal named: " + addedMetal.meltableIngredient.name + " at an amount of: " + addedMetal.amount);
                        currentContainedLiquidMetal.Add(addedMetal);
                    }
                }
                else
                {
                    Debug.Log("we melted an ingredient but the crucible was already full so it's contents were wasted. Uh oh");
                }

                onesToRemove.Add(m);
                Destroy(m.gameObject);
                
            }
        });

        currentInsertedMeltables = currentInsertedMeltables.Except(onesToRemove).ToList();
    }
    public void DecreaseHeat()
    {
        _CurrentHeat -= CoolSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IngredientInstance instance))
        {
            if(instance.Item.MeltedIngredient)
            {
                AttachIngredient(instance, other);
                instance.GetInteractable().selectEntered.AddListener(IngredientGrabbedOut);
            }
        }
    }

    void AttachIngredient(IngredientInstance instance, Collider instanceCollider)
    {
        instance.AttachedRB.isKinematic = true;
        instance.transform.parent = transform;

        foreach(Collider col in childColliders)
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

    [Serializable]
    struct ContainedLiquidMetal
    {
        public MeltableIngredientSO meltableIngredient;
        public int amount;
    }
}
