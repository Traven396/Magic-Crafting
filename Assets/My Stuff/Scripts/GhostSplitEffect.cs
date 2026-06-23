using UnityEngine;

public class GhostSplitEffect : MonoBehaviour
{
    public float xOffset;
    public float yOffset;

    public float mergeThreshold = .1f;

    //Ghost 1 is the one that will be offset by x and y, ghost 2 is the one that will be offset by -x and -y
    private GameObject ghost1;
    private GameObject ghost2;
    //Ghost 3 is the one that will be offset by x and -y, ghost 4 is the one that will be offset by -x and y
    private GameObject ghost3;
    private GameObject ghost4;

    private Renderer selfRenderer;
    private MeshFilter selfFilter;
    private void Start()
    {
        selfRenderer = GetComponent<Renderer>();
        selfFilter = GetComponent<MeshFilter>();

        selfRenderer.sharedMaterial.SetFloat("_StencilID", 1);

        ghost1 = CreateGhost("Ghost 1");
        ghost2 = CreateGhost("Ghost 2");
        ghost3 = CreateGhost("Ghost 3");
        ghost4 = CreateGhost("Ghost 4");

    }

    private GameObject CreateGhost(string name)
    {
        GameObject ghost = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));

        ghost.GetComponent<MeshFilter>().mesh = selfFilter.sharedMesh;
        ghost.GetComponent<MeshRenderer>().material = selfRenderer.sharedMaterial;
        ghost.transform.parent = transform;

        return ghost;
    }
    // Update is called once per frame
    void Update()
    {
        if (new Vector2(xOffset, yOffset).magnitude < mergeThreshold)
        {
            ghost1.SetActive(false);
            ghost2.SetActive(false);
            ghost3.SetActive(false);
            ghost4.SetActive(false);

            selfRenderer.enabled = true;
        }
        else
        {
            selfRenderer.enabled = false;

            ghost1.SetActive(true);
            ghost2.SetActive(true);
            ghost3.SetActive(true);
            ghost4.SetActive(true);
            ghost1.transform.position = transform.position + new Vector3(xOffset, yOffset, 0);
            ghost2.transform.position = transform.position + new Vector3(-xOffset, -yOffset, 0);
            ghost3.transform.position = transform.position + new Vector3(xOffset, -yOffset, 0);
            ghost4.transform.position = transform.position + new Vector3(-xOffset, yOffset, 0);
        }    
    }
}
