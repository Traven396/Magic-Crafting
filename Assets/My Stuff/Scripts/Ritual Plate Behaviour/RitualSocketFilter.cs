using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CenterSocketFilter : XRBaseTargetFilter
{
    public override bool canProcess { get { return isActiveAndEnabled; } }
    public string wantedTag;

    public override void Link(IXRInteractor interactor)
    {
        
    }

    public override void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
    {
        foreach (IXRInteractable target in targets) { 
            if(target.transform.tag.ToLower() == wantedTag.ToLower())
            {
                results.Add(target);
            }
        }
    }

    public override void Unlink(IXRInteractor interactor)
    {
        
    }

}
