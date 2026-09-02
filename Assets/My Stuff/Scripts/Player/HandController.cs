using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

public class HandController : MonoBehaviour
{
    [SerializeField] Transform VisualHand;
    [SerializeField] XRRayInteractor RayInteractor;


    XRInteractorLineVisual lineVisual;

    public void HideHand()
    {
        VisualHand.gameObject.SetActive(false);

        if(!RayInteractor.hasSelection)
        {
            RayInteractor.allowHover = false;
            RayInteractor.allowSelect = false;
        }
        
        if(!lineVisual)
        {
            lineVisual = RayInteractor.GetComponent<XRInteractorLineVisual>();
        }

        lineVisual.enabled = false;
    }

    public void ShowHand()
    {
        VisualHand.gameObject.SetActive(true);

        RayInteractor.allowHover = true;
        RayInteractor.allowSelect = true;

        if (!lineVisual)
        {
            lineVisual = RayInteractor.GetComponent<XRInteractorLineVisual>();
        }

        lineVisual.enabled = true;
    }
}
