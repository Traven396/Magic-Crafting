using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KilnCrucible : MonoBehaviour
{
    //This class will just be a container to hold all of the ingredients placed into it.
    //Have it work like in Hydroneer, where once its inside the crucible it cant collide with any of the other things inside of it, to save on space.
    //They should still be grabbable so you can get them out afterwards if needed.
    [SerializeField] int CoolSpeed = 1;
    [SerializeField] int CurrentHeat = 0;
    [SerializeField] Transform ColliderParent;

    bool isHeating;

    List<IngredientInstance> currentInsertedMeltables = new();

    Collider[] childColliders;

    private void Awake()
    {
        childColliders = ColliderParent.GetComponentsInChildren<Collider>();
    }

    private void FixedUpdate()
    {
        if (!isHeating)
        {
            if(CurrentHeat > 0)
                DecreaseHeat();
        }
    }

    public void BeginCooling()
    {
        isHeating = false;
    }

    public void IncreaseHeat(int heatAmount)
    {
        CurrentHeat += heatAmount;
        isHeating = true;
    }
    public void DecreaseHeat()
    {
        CurrentHeat -= CoolSpeed;
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

    
}
