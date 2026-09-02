using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

/// <summary>
/// Extends <see cref="XRGeneralGrabTransformer"/> with a spherical movement limit
/// measured from the object's position when the current grab begins.
/// </summary>
/// <remarks>
/// Inherits the general transformer's permitted displacement axis settings, including
/// <see cref="XRGeneralGrabTransformer.permittedDisplacementAxes"/> and
/// <see cref="XRGeneralGrabTransformer.constrainedAxisDisplacementMode"/>.
/// </remarks>
[AddComponentMenu("XR/Transformers/Distance Limited Grab Transformer")]
public class DistanceLimitedGrabTransformer : XRGeneralGrabTransformer
{
    [Header("Distance Limits")]
    [SerializeField, Min(0f)]
    [Tooltip("Maximum distance the object can move from its position when grabbed. Set to 0 to lock it in place.")]
    private float maximumDistance = 1f;

    [SerializeField]
    [Tooltip("When disabled, this transformer does not apply a distance limit.")]
    private bool distanceLimitEnabled = true;

    private Vector3 grabStartPosition;
    private int previousGrabCount;
    private bool hasGrabStartPosition;

    /// <summary>The position from which the current grab distance is measured.</summary>
    public Vector3 GrabStartPosition => grabStartPosition;

    /// <summary>The currently configured maximum movement distance.</summary>
    public float MaximumDistance => maximumDistance;

    /// <summary>Whether the distance limit is currently applied.</summary>
    public bool DistanceLimitEnabled => distanceLimitEnabled;


    /// <summary>
    /// Sets and enables the maximum movement distance for future and active grabs.
    /// </summary>
    public void SetDistanceLimit(float distance)
    {
        maximumDistance = Mathf.Max(0f, distance);
        distanceLimitEnabled = true;
    }

    /// <summary>Disables the distance limit without changing its configured value.</summary>
    public void ClearDistanceLimit()
    {
        distanceLimitEnabled = false;
    }

    /// <summary>
    /// Sets the point used to measure the movement limit during the current grab.
    /// </summary>
    public void SetGrabStartPosition(Vector3 position)
    {
        grabStartPosition = position;
        hasGrabStartPosition = true;
    }
    /// <inheritdoc />
    public override void OnGrab(XRGrabInteractable grabInteractable)
    {
        base.OnGrab(grabInteractable);


        // XRGeneralGrabTransformer re-runs OnGrab when a two-handed grab returns to
        // one hand. Preserve the original position for the whole selection instead.

        if (previousGrabCount == 0)
            SetGrabStartPosition(grabInteractable.transform.position); 
        

        previousGrabCount = grabInteractable.interactorsSelecting.Count; 
    }

    /// <inheritdoc />
    public override void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
    {
        base.OnGrabCountChanged(grabInteractable, targetPose, localScale);

        var currentGrabCount = grabInteractable.interactorsSelecting.Count;
        if (currentGrabCount == 0)
            hasGrabStartPosition = false;

        previousGrabCount = currentGrabCount;
    }

    /// <inheritdoc />
    public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
    {
        base.Process(grabInteractable, updatePhase, ref targetPose, ref localScale);

        if (!distanceLimitEnabled || !hasGrabStartPosition ||
            (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic && updatePhase != XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender))
            return;

        targetPose.position = Vector3.MoveTowards(grabStartPosition, targetPose.position, maximumDistance);
    }
}
