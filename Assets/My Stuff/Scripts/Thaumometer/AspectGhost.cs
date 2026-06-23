using NUnit.Framework.Constraints;
using UnityEngine;

public class AspectGhost : MonoBehaviour
{
    public float ghostOffset;

    public float mergeThreshold = .1f;
    public float fadeDistance = 1.0f;

    private Ghost ghost1;
    private Ghost ghost2;
    private Ghost ghost3;

    private Renderer selfRenderer;
    private MeshFilter selfFilter;
    public Vector2 HiddenLocation;

    private void Start()
    {
        selfRenderer = GetComponent<Renderer>();
        selfFilter = GetComponent<MeshFilter>();

        selfRenderer.sharedMaterial.SetFloat("_StencilID", 1);

        ghost1 = CreateGhost("Ghost 1");
        ghost2 = CreateGhost("Ghost 2");
        ghost3 = CreateGhost("Ghost 3");


        //make a method that gets called by the thaumometer brain and passes through the last discovered aspect, then randomly generate the next one a distance away from it.
        //if this method gets passed through a null then it can just place it anywhere that isnt close to the center

    }

    public void RandomizeLocation(Vector2? lastLocation)
    {
        if(lastLocation != null)
        {
            while (true)
            {
                var sampleHiddenLocation = new Vector2(Random.Range(-15f, 16), Random.Range(-15f, 16));
                if (Mathf.Abs(sampleHiddenLocation.x - lastLocation.Value.x) < 1.5f && Mathf.Abs(sampleHiddenLocation.y - lastLocation.Value.y) < 1.5f)
                {
                    continue;
                }
                else
                {
                    HiddenLocation = sampleHiddenLocation;
                    break;
                }
            }
        }
        else
        {
            var sampleHiddenLocation = new Vector2(Random.Range(-15f, 16), Random.Range(-15f, 16));
            if(sampleHiddenLocation.x <= 1.5f && sampleHiddenLocation.x >= -1.5f && sampleHiddenLocation.y <= 1.5f && sampleHiddenLocation.y >= -1.5f)
            {
                RandomizeLocation(null);
            }
            else
            {
                HiddenLocation = sampleHiddenLocation;
            }
        }

        HiddenLocation = new(Mathf.Round(HiddenLocation.x * 10f) * .1f, Mathf.Round(HiddenLocation.y * 10f) * .1f);
    }
    public void SetOffset(float offset)
    {
        ghostOffset = offset;
    }
    private Ghost CreateGhost(string name)
    {
        GameObject ghost = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));

        ghost.GetComponent<MeshFilter>().mesh = selfFilter.sharedMesh;
        ghost.GetComponent<MeshRenderer>().material = selfRenderer.sharedMaterial;
        
        ghost.transform.localScale = transform.localScale;

        ghost.transform.parent = transform;

        Ghost newGhost = new(ghost);

        return newGhost;
    }
    void Update()
    {
        if (ghostOffset < mergeThreshold)
        {
            ghost1.SetActive(false);
            ghost2.SetActive(false);
            ghost3.SetActive(false);

            selfRenderer.enabled = true;
        }
        else
        {
            selfRenderer.enabled = false;

            ghost1.SetActive(true);
            ghost2.SetActive(true);
            ghost3.SetActive(true);

            ghost1.transform.position = transform.TransformPoint(new Vector3(ghostOffset, 0, 0));
            ghost2.transform.position = transform.TransformPoint(new Vector3(-ghostOffset, 0, 0));
            ghost3.transform.position = transform.TransformPoint(new Vector3(0, ghostOffset, 0));

            ghost1.SetOpacity(1 - (ghostOffset - mergeThreshold) / fadeDistance);
            ghost2.SetOpacity(1 - (ghostOffset - mergeThreshold) / fadeDistance);
            ghost3.SetOpacity(1 - (ghostOffset - mergeThreshold) / fadeDistance);
        }
    }

    class Ghost
    {
        public MeshRenderer renderer;
        public Transform transform;

        public Ghost (GameObject obj)
        {
            renderer = obj.GetComponent<MeshRenderer>();
            transform = obj.transform;
        }

        public void SetActive(bool active)
        {
            renderer.enabled = active;
        }

        public void SetOpacity(float opacity)
        {
            Color color = renderer.material.color;
            color.a = opacity;
            renderer.material.color = color;
        }
    }
}
