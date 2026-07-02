using Alchemy.Inspector;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Tongs : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] float OpenAngle = 30f;
    [Title("References")]
    [SerializeField] Transform TongLeft;
    [SerializeField] Transform TongRight;
    [SerializeField] XRDirectInteractor Interactor;


    Sequence tongSequence;

    private void Start()
    {

        //Need to set the tongs to an open state
        TongLeft.localRotation = Quaternion.Euler(0, OpenAngle, 0);
        TongRight.localRotation = Quaternion.Euler(0, -OpenAngle, 0);

        tongSequence = DOTween.Sequence();

        tongSequence.Append(TongLeft.DOLocalRotate(Vector3.zero, 0.25f));
        tongSequence.Join(TongRight.DOLocalRotate(Vector3.zero, 0.25f));

        tongSequence.SetAutoKill(false);
        tongSequence.Pause();
    }



    [Button]
    public void TongOpen()
    {
        tongSequence.PlayBackwards();

        if (Interactor.hasSelection)
        {
            Interactor.interactionManager.SelectExit(Interactor, Interactor.firstInteractableSelected);
        }
    }
    [Button]
    public void TongClose()
    {
        tongSequence.PlayForward();

        //Non-Visual stuff
        var potentialInteractables = Interactor.interactablesHovered;

        if (potentialInteractables.Any())
        {
            Debug.Log("There was a possible interactable");
            Interactor.interactionManager.SelectEnter(Interactor, potentialInteractables.First() as IXRSelectInteractable);
        }

    }
}
