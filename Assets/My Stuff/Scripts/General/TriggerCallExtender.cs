using UnityEngine;

public class TriggerCallExtender : MonoBehaviour
{
    [SerializeField] private Component Recipient;
    private ITriggerable _recipient;

    private void Awake()
    {
        if (Recipient is ITriggerable)
            _recipient = Recipient as ITriggerable;
        else
            Debug.LogError(gameObject.name + " does not a valid ITriggerable reference. Please fix that bud");
    }

    private void OnTriggerEnter(Collider other)
    {
        _recipient?.OnTriggerEnterCall(other);
    }

    private void OnTriggerStay(Collider other)
    {
        _recipient?.OnTriggerStayCall(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _recipient?.OnTriggerExitCall(other);
    }
}

public interface ITriggerable
{
    public void OnTriggerEnterCall(Collider other);
    public void OnTriggerStayCall(Collider other);
    public void OnTriggerExitCall(Collider other);
}
