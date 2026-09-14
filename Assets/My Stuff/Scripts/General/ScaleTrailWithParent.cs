using UnityEngine;

public class ScaleTrailWithParent : MonoBehaviour
{
    private TrailRenderer trailRenderer;
    private float initialWidth;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        initialWidth = trailRenderer.widthMultiplier;
    }

    void Update()
    {
        if (transform.parent != null)
        {
            // Assumes uniform scaling on the parent
            float parentScale = transform.parent.localScale.x;
            trailRenderer.widthMultiplier = initialWidth * parentScale;
        }
    }
}
