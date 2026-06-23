using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class IngredientSocketFilter : XRBaseTargetFilter
{
    public override void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
    {
        foreach (IXRInteractable target in targets)
        {
            if (target.transform.TryGetComponent(out IngredientInstance instance))
            {
                if (instance.Item.Discoverable())
                {
                    results.Add(target); 
                }
            }
        }
    }
}
