using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ProtoWand : MonoBehaviour
{
    //This will be the holder class that will keep reference to all of our current stats and everything

    //What spell gem do we have socketed
    //What wand frame is currently attached
    //What ritual is about to be performed to us

    //This will also be how we generate the final wand prefab and replace it.


    [SerializeField] XRSocketInteractor WandFrameSocket;
    public Mesh CoreMesh;
    public Material CoreMaterial;

    public WandFrame _AttachedWandFrame { private set; get; }
    public SpellGem _AttachedSpellGem { private set; get; }

    XRGrabInteractable selfInteractable;
    FixedJoint wandFrameJoint;
    public bool ValidWand => _AttachedWandFrame && _AttachedSpellGem;
    private void OnEnable()
    {
        selfInteractable = GetComponent<XRGrabInteractable>();

        WandFrameSocket.selectEntered.AddListener(WandFrameAttached);
        WandFrameSocket.selectExited.AddListener(WandFrameDetached);
    }

    private void OnDisable()
    {
        WandFrameSocket.selectEntered.RemoveListener(WandFrameAttached);
        WandFrameSocket.selectExited.RemoveListener(WandFrameDetached);
    }

    private void OnDestroy()
    {
        if(_AttachedWandFrame)
            Destroy(_AttachedWandFrame.gameObject);

        if(_AttachedSpellGem)
            Destroy(_AttachedSpellGem.gameObject);
    }

    void WandFrameAttached(SelectEnterEventArgs args)
    {
        _AttachedWandFrame = args.interactableObject.transform.GetComponent<WandFrame>();

        _AttachedWandFrame.GemSocket.selectEntered.AddListener(SpellGemAttach);
        _AttachedWandFrame.GemSocket.selectExited.AddListener(SpellGemDetach);

        _AttachedWandFrame.transform.position = WandFrameSocket.attachTransform.position;
        _AttachedWandFrame.transform.rotation = WandFrameSocket.attachTransform.rotation;

        wandFrameJoint = gameObject.AddComponent<FixedJoint>();

        wandFrameJoint.autoConfigureConnectedAnchor = false;
        wandFrameJoint.connectedAnchor = Vector3.zero;

        wandFrameJoint.connectedBody = _AttachedWandFrame.GetComponent<Rigidbody>();
        wandFrameJoint.enableCollision = false;


        selfInteractable.interactionManager.RegisterParentRelationship(args.interactableObject, selfInteractable);

        if (_AttachedWandFrame.GemSocket.hasSelection)
        {
            _AttachedSpellGem = _AttachedWandFrame.GemSocket.firstInteractableSelected.transform.GetComponent<SpellGem>();
        }
    }
    void WandFrameDetached(SelectExitEventArgs args)
    {
        _AttachedWandFrame.GemSocket.selectEntered.RemoveListener(SpellGemAttach);
        _AttachedWandFrame.GemSocket.selectExited.RemoveListener(SpellGemDetach);

        Destroy(wandFrameJoint);

        selfInteractable.interactionManager.UnregisterParentRelationship(args.interactableObject, selfInteractable);

        _AttachedSpellGem = null;
        _AttachedWandFrame = null;
    }

    void SpellGemAttach(SelectEnterEventArgs args) 
    {
        _AttachedSpellGem = _AttachedWandFrame.GemSocket.firstInteractableSelected.transform.GetComponent<SpellGem>();
    }
    void SpellGemDetach(SelectExitEventArgs args)
    {
        _AttachedSpellGem = null;
    }
}
