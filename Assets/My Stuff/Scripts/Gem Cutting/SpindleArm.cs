using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable), typeof(DistanceLimitedGrabTransformer))]
public class SpindleArm : MonoBehaviour
{
    [SerializeField] Transform GrindingSurface;
    [Header("Grinding Settings")]
    [SerializeField] float MaxGrindOverlap = 0.01f;
    [SerializeField] float GrindingSpeed = 0.01f;

    Rigidbody rb;
    Collider[] childColliders;
    XRGrabInteractable grabInteractable;

    DistanceLimitedGrabTransformer distanceLimiter;
    DeformableGem currentInsertedGem;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        childColliders = rb.GetComponentsInChildren<Collider>(true);

        grabInteractable = GetComponent<XRGrabInteractable>();
        distanceLimiter = GetComponent<DistanceLimitedGrabTransformer>();

        // The distance limiter already performs the full XRGeneralGrabTransformer
        // behavior. Do not add a second default transformer that could overwrite it.
        grabInteractable.addDefaultGrabTransformers = false;
    }

    //Gem inserted into the socket
    public void OnGemInserted(SelectEnterEventArgs args)
    {
        var interactableCollider = args.interactableObject.transform.GetComponent<Collider>();
        
        foreach (Collider collider in childColliders) 
        {
            Physics.IgnoreCollision(interactableCollider, collider, true);
        }

        currentInsertedGem = args.interactableObject.transform.GetComponent<DeformableGem>();
        if (currentInsertedGem == null)
            return;

        currentInsertedGem.SocketEntered(GrindingSurface, MaxGrindOverlap, GrindingSpeed);
        UpdateDistanceLimit();
    }
    //Gem leaves the socket
    public void OnGemRemoved(SelectExitEventArgs args)
    {
        var interactableCollider = args.interactableObject.transform.GetComponent<Collider>();

        foreach (Collider collider in childColliders)
        {
            Physics.IgnoreCollision(interactableCollider, collider, false);
        }

        if (currentInsertedGem != null)
            currentInsertedGem.SocketExit();

        currentInsertedGem = null;
        if (distanceLimiter != null)
            distanceLimiter.ClearDistanceLimit();
    }

    private void FixedUpdate()
    {
        if (currentInsertedGem)
        {
            UpdateDistanceLimit();
        }
    }

    /// <summary>
    /// Caps the arm at the position where the gem's foremost vertex is exactly at the
    /// permitted surface penetration. Grinding moves that vertex back, increasing the
    /// available travel on the next physics update.
    /// </summary>
    void UpdateDistanceLimit()
    {
        if (distanceLimiter == null || GrindingSurface == null)
            return;

        float closestVertexZ = currentInsertedGem.GetClosestVertexZ(GrindingSurface);

        // Local negative Z is the inside of the surface. Grind only after contact;
        // the transformer still prevents the gem from moving past MaxGrindOverlap.
        currentInsertedGem.Grinding = closestVertexZ < 0f;

        // Translating the arm by this amount toward the wheel places its closest
        // vertex at -MaxGrindOverlap in the GrindingSurface's local space.
        float forwardTravelToLimit = closestVertexZ + MaxGrindOverlap;
        Vector3 limitPosition = transform.position - GrindingSurface.forward * forwardTravelToLimit;

        distanceLimiter.SetDistanceLimit(Vector3.Distance(distanceLimiter.GrabStartPosition, limitPosition));
    }

    
}
