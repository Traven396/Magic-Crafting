using System;
using System.Linq;
using Unity.XR.CoreUtils.GUI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[Serializable]
public class InteractorTypeFilter : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
{
    [Flags]
    enum InteractorType
    {
        Other = 0,
        Direct = 1,
        Ray = 2,
        Socket = 4,
        Poke = 8
    }

    
    [SerializeField]
    private InteractorType allowedInteractors;

    public bool canProcess => enabled;

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        //We need to convert the interactor coming in to a bit flipped thing so that we can compare it to the results
        InteractorType currentInteractor = InteractorType.Other;

        if (interactor.GetType() == typeof(XRDirectInteractor))
        {
            currentInteractor = InteractorType.Direct;
        }
        else if (interactor.GetType() == typeof(XRRayInteractor))
        {
            currentInteractor = InteractorType.Ray;
        }
        else if (interactor.GetType() == typeof(XRSocketInteractor))
        {
            currentInteractor = InteractorType.Socket;
        }
        else if (interactor.GetType() == typeof(XRPokeInteractor))
        {
            currentInteractor = InteractorType.Poke;
        }



        if (allowedInteractors.HasFlag(currentInteractor))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        InteractorType currentInteractor = InteractorType.Other;

        if (interactor.GetType() == typeof(XRDirectInteractor))
        {
            currentInteractor = InteractorType.Direct;
        }
        else if (interactor.GetType() == typeof(XRRayInteractor))
        {
            currentInteractor = InteractorType.Ray;
        }
        else if (interactor.GetType() == typeof(XRSocketInteractor))
        {
            currentInteractor = InteractorType.Socket;
        }
        else if (interactor.GetType() == typeof(XRPokeInteractor))
        {
            currentInteractor = InteractorType.Poke;
        }



        if (allowedInteractors.HasFlag(currentInteractor))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
