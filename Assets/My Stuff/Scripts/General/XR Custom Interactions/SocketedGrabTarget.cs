using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Keeps an item locked while a socket holds it. A free direct hand can arm the item
/// with its Activate input, then use its normal Select input to take it from the socket.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(XRGrabInteractable))]
public class SocketedGrabTarget : MonoBehaviour, IXRSelectFilter
{
    [Header("Ghost Visual")]
    [SerializeField] private Material ghostMaterial;
    [SerializeField, Min(1f)] private float ghostScale = 1.06f;

    private XRGrabInteractable grabInteractable;
    private XRInteractionManager interactionManager;
    private XRSocketInteractor containingSocket;
    private XRGrabInteractable containingInteractable;
    private XRDirectInteractor armedInteractor;
    private GameObject ghostVisual;

    public bool canProcess => enabled;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        interactionManager = grabInteractable.interactionManager;

        if (interactionManager == null)
            interactionManager = FindFirstObjectByType<XRInteractionManager>();

        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);

        CreateGhostVisual();
        SetGhostVisible(false);
    }

    private void OnDestroy()
    {
        if (grabInteractable == null)
            return;

        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void Update()
    {
        if (containingSocket == null)
            return;

        if (armedInteractor != null)
        {
            if (!CanArm(armedInteractor))
                Disarm();

            return;
        }

        foreach (var directInteractor in FindObjectsByType<XRDirectInteractor>(FindObjectsSortMode.None))
        {
            if (!CanArm(directInteractor))
                continue;

            if (directInteractor.activateInput.ReadWasPerformedThisFrame())
            {
                armedInteractor = directInteractor;
                SetGhostVisible(true);
                break;
            }
        }
    }

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        if (containingSocket == null)
            return true;

        if (interactor is XRSocketInteractor castInteractor)
        {
            if(castInteractor == containingSocket)
                return true;
        }

        if (interactor is not XRDirectInteractor directInteractor || directInteractor != armedInteractor)
            return false;

        if (!CanArm(directInteractor) || interactionManager == null)
            return false;

        // The player's normal Select input caused this filter check. Release the socket
        // immediately, then let XRI complete that same Select operation on the direct hand.

        ReleaseFromSocket();
        return true;
    }

    private bool CanArm(XRDirectInteractor directInteractor)
    {
        if (containingSocket == null || directInteractor == null)
            return false;

        if (!directInteractor.IsHovering(grabInteractable))
            return false;

        // The hand holding the parent wand may never arm or extract its attached part.
        if (containingInteractable != null && containingInteractable.interactorsSelecting.Contains(directInteractor))
            return false;

        return true;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor socketInteractor)
        {
            containingSocket = socketInteractor;
            containingInteractable = socketInteractor.GetComponentInParent<XRGrabInteractable>();
            Disarm();

            GetComponent<Rigidbody>().isKinematic = false;
            return;
        }

        if (args.interactorObject is XRDirectInteractor)
            Disarm();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (ReferenceEquals(args.interactorObject, containingSocket))
        {
            containingSocket = null;
            containingInteractable = null;
            Disarm();
        }
    }

    private void ReleaseFromSocket()
    {
        var socketToRelease = containingSocket;
        containingSocket = null;
        containingInteractable = null;
        Disarm();

        interactionManager.SelectExit((IXRSelectInteractor)socketToRelease, (IXRSelectInteractable)grabInteractable);
    }

    private void Disarm()
    {
        armedInteractor = null;
        SetGhostVisible(false);
    }

    private void CreateGhostVisual()
    {
        if (ghostMaterial == null)
        {
            Debug.LogWarning($"{name} has no ghost material assigned.", this);
            return;
        }

        ghostVisual = new GameObject("Socket Grab Ghost");
        ghostVisual.transform.SetParent(transform, false);
        ghostVisual.transform.localScale = Vector3.one * ghostScale;

        foreach (var sourceFilter in GetComponentsInChildren<MeshFilter>(true))
        {
            if (sourceFilter.sharedMesh == null || sourceFilter.transform.parent != transform)
                continue;

            var ghostPart = new GameObject($"{sourceFilter.name} Ghost");

            ghostPart.transform.SetParent(ghostVisual.transform, false);

            ghostPart.transform.localPosition = Vector3.zero;
            ghostPart.transform.localRotation = Quaternion.identity;

            ghostPart.transform.localScale = sourceFilter.transform.localScale;

            ghostPart.AddComponent<MeshFilter>().sharedMesh = sourceFilter.sharedMesh;
            var ghostRenderer = ghostPart.AddComponent<MeshRenderer>();
            ghostRenderer.sharedMaterial = ghostMaterial;
            ghostRenderer.shadowCastingMode = ShadowCastingMode.Off;
            ghostRenderer.receiveShadows = false;
        }
    }

    private void SetGhostVisible(bool visible)
    {
        if (ghostVisual != null && ghostVisual.activeSelf != visible)
            ghostVisual.SetActive(visible);
    }
}
