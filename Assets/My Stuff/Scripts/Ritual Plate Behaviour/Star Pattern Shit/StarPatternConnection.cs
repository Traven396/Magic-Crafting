using UnityEngine;

public class StarPatternConnection : MonoBehaviour
{
    [SerializeField] string FingerTag;
    [SerializeField][Range(0, 1)] float SwipeThreshold;

    Vector3 initialDirection;
    Transform currentFinger;
    StarGridManager gridManager;

    TagHandle FingerTagHandle;

    private void Awake()
    {
        FingerTagHandle = TagHandle.GetExistingTag(FingerTag);
    }
    public void SpawnMethod(StarGridManager yup)
    {
        gridManager = yup;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(FingerTagHandle))
        {
            if (!currentFinger)
            {
                currentFinger = other.transform;
                initialDirection = (currentFinger.position - transform.position).normalized;

                //Check if the finger entered close to the edges of the collider. Dot Product of direction compared to the Up vector checks how close it is to pointing up
                //We use Abs so that it can check both ends, basically the whole local Y axis
                if(Mathf.Abs(Vector3.Dot(initialDirection, transform.up)) >= 0.9f)
                {
                    currentFinger = null;
                    initialDirection = Vector3.zero;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentFinger)
        {
            if (other.transform == currentFinger)
            { 
                Vector3 exitDirection = currentFinger.position - transform.position;

                if(Vector3.Dot(initialDirection, exitDirection) <= SwipeThreshold)
                {
                    gridManager.RemoveConnectionLine(transform.parent.gameObject);
                }

                currentFinger = null;
            }
        }
    }
}
