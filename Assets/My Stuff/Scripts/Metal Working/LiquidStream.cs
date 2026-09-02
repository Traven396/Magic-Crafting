using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LiquidStream : MonoBehaviour
{
    LineRenderer lineRenderer = null;

    Vector3 targetPosition = Vector3.zero;

    Coroutine pourCoroutine;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    public void Begin()
    {
        pourCoroutine = StartCoroutine(BeginPouring());
    }
    public void End()
    {
        StopCoroutine(pourCoroutine);
        pourCoroutine = StartCoroutine(StopPouring());
    }

    IEnumerator StopPouring()
    {
        while(!HasReachedPosition(0, targetPosition))
        {
            AnimateToPosition(0, targetPosition);

            AnimateToPosition(1, targetPosition);

            yield return null;
        }
        Destroy(gameObject);
    }
    IEnumerator BeginPouring()
    {
        while (gameObject.activeSelf)
        {
            targetPosition = FindEndPoint();

            MoveToPosition(0, transform.position);
            AnimateToPosition(1, targetPosition);


            yield return null;
        }

    }

    Vector3 FindEndPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        Physics.Raycast(ray, out hit, 2f);
        
        //If the ray actually hit something, we return that point. Otherwise we just return the end of the raycast since it is length 2
        Vector3 endPoint = hit.collider ? hit.point : ray.GetPoint(2);


        return endPoint;
    }

    void MoveToPosition(int index, Vector3 targetPosition)
    {
        lineRenderer.SetPosition(index, targetPosition);
    }

    void AnimateToPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPoint = lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * 1.75f);

        lineRenderer.SetPosition(index, newPosition);
    }

    bool HasReachedPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPositon = lineRenderer.GetPosition(index);

        return currentPositon == targetPosition;
    }

    
}
