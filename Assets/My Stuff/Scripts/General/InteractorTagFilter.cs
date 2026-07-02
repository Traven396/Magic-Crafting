using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InteractorTagFilter : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
{
    [SerializeField] string[] allowedTags;

    public bool canProcess => enabled;

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        if(allowedTags.Where(tag => interactor.transform.CompareTag(tag)).Any())
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
        if (allowedTags.Where(tag => interactor.transform.CompareTag(tag)).Any())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
