using UnityEngine;

public class DeformableGem : MonoBehaviour
{
    [SerializeField] float Quality;
    public GemCutPlate.DebugCutTypes DesiredCut;
    //Local variable saves of the spindle arm settings.
    float grindingSpeed = 0.01f;
    float maxPenetration = 0.02f;

    [HideInInspector] public MeshFilter SelfFilter;

    Transform grindingWheelSurface;

    Mesh selfMesh;
    Vector3[] vertices;

    float visualUpdateTimer = 0;

    [HideInInspector] public bool CanGrind = false;
    [HideInInspector] public bool Grinding = false;

    private void Awake()
    {
        SelfFilter = GetComponent<MeshFilter>();

        selfMesh = SelfFilter.mesh;

        vertices = selfMesh.vertices;

        selfMesh.MarkDynamic();
    }
    

    private void Update()
    {
        if (CanGrind)
        {
            if (Grinding)
            {
                visualUpdateTimer += Time.deltaTime;

                if (visualUpdateTimer < 1 / 30)
                {
                    //We only do the mesh deformation at 30hz to save performance
                    //Once this timer goes above 1/30 then we do 1 pass then reset it
                    return;
                }

                visualUpdateTimer = 0;
                if (grindingWheelSurface)
                    GrindGem(grindingWheelSurface);
            } 
        }
    }
    public void SocketEntered(Transform newGrindingWheel, float maxOverlap, float speed)
    {
        //Save a local reference to the grinding wheel
        grindingWheelSurface = newGrindingWheel;

        maxPenetration = maxOverlap;

        grindingSpeed = speed;
    }
    public void SocketExit()
    {
        Grinding = false;
        CanGrind = false;
        grindingWheelSurface = null;

        maxPenetration = 0;
        grindingSpeed = 0;
    }

    /// <summary>
    /// Gets the local Z position of the gem vertex furthest into the grinding surface.
    /// A negative result means that the gem overlaps the surface plane.
    /// </summary>
    public float GetClosestVertexZ(Transform comparisonTransform)
    {
        float closestZ = float.MaxValue;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosition = transform.TransformPoint(vertices[i]);
            float wheelRelativeZ = comparisonTransform.InverseTransformPoint(worldPosition).z;
            closestZ = Mathf.Min(closestZ, wheelRelativeZ);
        }

        return closestZ;
    }

    #region Gem Quality
    public void SetGemQuality(float quality)
    {
        Quality = quality;
    }
    public float GetGemQuality() {return Quality; }
    
    #endregion
    void GrindGem(Transform grindSurface)
    {
        bool dirtyMesh = false;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Convert vertex from gem-local space
            // into world space.
            Vector3 worldPosition = transform.TransformPoint(vertices[i]);

            // Convert world position into
            // grinding-surface local space.
            Vector3 wheelRelativePosition = grindSurface.InverseTransformPoint(worldPosition);

            // Not inside the grinding surface.
            if (wheelRelativePosition.z >= 0f)
                continue;

            // Outside the physical width of the wheel.
            if (Mathf.Abs(wheelRelativePosition.x) > 0.3f)
                continue;

            // Outside the physical height of the wheel.
            if (Mathf.Abs(wheelRelativePosition.y) > 0.3f)
                continue;

            // How deeply is the vertex penetrating?
            float pressure = Mathf.Clamp01(-wheelRelativePosition.z / maxPenetration);

            // Amount of material to remove this frame.
            float removal = grindingSpeed * pressure * Time.deltaTime;

            wheelRelativePosition.z += removal;

            worldPosition = grindSurface.TransformPoint(wheelRelativePosition);

            vertices[i] = transform.InverseTransformPoint(worldPosition);

            dirtyMesh = true;
        }


        //Re-set the vertices to the mesh
        if (dirtyMesh)
        {
            VisualUpdateMesh();
        }
    }

    void VisualUpdateMesh()
    {
        selfMesh.vertices = vertices;

        selfMesh.RecalculateNormals();

        selfMesh.RecalculateBounds();
    }

}
