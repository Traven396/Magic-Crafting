using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static GemCutPlate;

public class GemCuttingApparatus : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] XRSocketInteractor SpindleSocket;
    [SerializeField] XRSocketInteractor GuideSocket;
    [SerializeField] CustomXRKnob XAxisKnob, YAxisKnob;
    [SerializeField] RotateTransformOverTime WheelRotater;

    [Header("Project References")]
    [SerializeField] GameObject VisualGuidePrefab;
    [SerializeField] Mesh RoundMesh, SquareMesh, TriangleMesh;

    [Header("Gem Rotation")]
    [SerializeField, Min(0f)] float GemRotationSpeed = 180f;

    [Header("Quality")]

    [Tooltip("Surface error at or below this distance is considered essentially perfect.")]
    [SerializeField]
    private float perfectTolerance = 0.001f;

    [Tooltip("Surface error at or above this distance results in a score of zero.")]
    [SerializeField]
    private float failureDistance = 0.025f;

    SpindleArm _SpindleArm;
    DeformableGem _CurrentInsertedGem;

    GameObject spawnedVisualGuide;
    MeshFilter spawnedVisualsFilter;
    // Keep the dial state as floats. Do not derive it from transform.eulerAngles:
    // Unity wraps those values to 0-360, which loses the direction of a turn.
    float xRotation;
    float yRotation;

    Quaternion DialRotation => Quaternion.Euler(xRotation * 360f, yRotation * 360f, 0f);
    DebugCutTypes _CurrentCut;

    private void OnEnable()
    {
        WheelRotater.PauseRotation = true;

        _SpindleArm = GetComponentInChildren<SpindleArm>();

        GuideSocket.selectEntered.AddListener(NewGuidePlateAttached);
        GuideSocket.selectExited.AddListener(GuidePlateRemoved);

        SpindleSocket.selectEntered.AddListener(GemInserted);
        SpindleSocket.selectExited.AddListener(GemRemoved);

        XAxisKnob.onValueChange.AddListener(RotateGemX);
        YAxisKnob.onValueChange.AddListener(RotateGemY);

    }

    private void OnDisable()
    {
        GuideSocket.selectEntered.RemoveListener(NewGuidePlateAttached);
        GuideSocket.selectExited.RemoveListener(GuidePlateRemoved);

        SpindleSocket.selectEntered.RemoveListener(GemInserted);
        SpindleSocket.selectExited.RemoveListener(GemRemoved);

        XAxisKnob.onValueChange.RemoveListener(RotateGemX);
        YAxisKnob.onValueChange.RemoveListener(RotateGemY);

    }

    private void Update()
    {

        SpindleSocket.attachTransform.rotation = Quaternion.RotateTowards(SpindleSocket.attachTransform.rotation, DialRotation, GemRotationSpeed * Time.deltaTime);


    }


    void GemInserted(SelectEnterEventArgs args)
    {
        _CurrentInsertedGem = args.interactableObject.transform.GetComponent<DeformableGem>();

        _SpindleArm.OnGemInserted(args);

        _CurrentInsertedGem.transform.rotation = SpindleSocket.attachTransform.rotation;

        if (GuideSocket.hasSelection)
            _CurrentInsertedGem.CanGrind = true;
    }
    void GemRemoved(SelectExitEventArgs args) 
    {
        if(GuideSocket.hasSelection)
            _CurrentInsertedGem.SetGemQuality(CalculateShapeAccuracy());

        _CurrentInsertedGem = null;

        _SpindleArm.OnGemRemoved(args);
    }
   

    void NewGuidePlateAttached(SelectEnterEventArgs args)
    {
        WheelRotater.PauseRotation = false;

        if (!spawnedVisualGuide)
        {
            spawnedVisualGuide = Instantiate(VisualGuidePrefab, SpindleSocket.attachTransform);

            spawnedVisualsFilter = spawnedVisualGuide.GetComponent<MeshFilter>();

            spawnedVisualGuide.layer = LayerMask.NameToLayer("RenderAbove");
        }

        spawnedVisualGuide.SetActive(true);
        //We need to set the rotation of it to whatever the spindle arm's setting would be
        //Then update the mesh to whatever the new one is

        //spawnedVisualGuide.transform.rotation = DialRotation;

        DebugCutTypes Blehh = args.interactableObject.transform.GetComponent<GemCutPlate>().CutType;

        switch (Blehh)
        {
            case DebugCutTypes.Round:

                ChangeGuideMesh(RoundMesh);

                break;
            case DebugCutTypes.Square:

                ChangeGuideMesh(SquareMesh);

                break;
            case DebugCutTypes.Triangle:

                ChangeGuideMesh(TriangleMesh);

                break;
        }

        _CurrentCut = Blehh;

        if (_CurrentInsertedGem)
        {
            
            _CurrentInsertedGem.CanGrind = true;
        }
    }

    void GuidePlateRemoved(SelectExitEventArgs args)
    {
        if(_CurrentInsertedGem)
        { 
            _CurrentInsertedGem.SetGemQuality(CalculateShapeAccuracy());
            _CurrentInsertedGem.CanGrind = false;
        }

        spawnedVisualGuide.SetActive(false);

        WheelRotater.PauseRotation = true;
    }


    void ChangeGuideMesh(Mesh newGuideMesh)
    {
        //We only ever have 1 guide and we just change the mesh that it is displaying for simplicity
        spawnedVisualsFilter.mesh = newGuideMesh;
    }
    public void RotateGemX(float newRotation)
    {
        xRotation = newRotation;
    }

    public void RotateGemY(float newRotation)
    {
        yRotation = newRotation;
    }

    #region Gem Quality Calculation
    

    /// <summary>
    /// Calculates the current Shape Accuracy of the finished gem.
    /// Returns a value from 0 to 100.
    /// </summary>
    public float CalculateShapeAccuracy()
    {
        if (!_CurrentInsertedGem || !spawnedVisualsFilter)
            return 0;

        float finalToTarget = CalculateAverageSurfaceDistance(_CurrentInsertedGem.SelfFilter, spawnedVisualsFilter);

        float targetToFinal = CalculateAverageSurfaceDistance(spawnedVisualsFilter, _CurrentInsertedGem.SelfFilter);

        float error = (finalToTarget + targetToFinal) * 0.5f;

        _CurrentInsertedGem.DesiredCut = _CurrentCut;

        return ErrorToScore(error);
    }

    /// <summary>
    /// Calculates the average distance from every vertex
    /// of one mesh to the closest point on the surface
    /// of another mesh.
    /// </summary>
    private float CalculateAverageSurfaceDistance(MeshFilter source, MeshFilter destination)
    {
        Vector3[] sourceVertices = source.sharedMesh.vertices;

        Vector3[] destinationVertices = destination.sharedMesh.vertices;

        int[] destinationTriangles = destination.sharedMesh.triangles;

        if (sourceVertices.Length == 0 || destinationVertices.Length == 0 || destinationTriangles.Length < 3)
        {
            return float.MaxValue;
        }

        float totalDistance = 0f;

        for (int i = 0; i < sourceVertices.Length; i++)
        {
            Vector3 worldPoint = source.transform.TransformPoint(sourceVertices[i]);

            float closestDistance = float.MaxValue;

            // Test the point against every triangle
            // in the destination mesh.
            for (int t = 0; t < destinationTriangles.Length; t += 3)
            {
                Vector3 a = destination.transform.TransformPoint(destinationVertices[destinationTriangles[t]]);

                Vector3 b = destination.transform.TransformPoint(destinationVertices[destinationTriangles[t + 1]]);

                Vector3 c = destination.transform.TransformPoint(destinationVertices[destinationTriangles[t + 2]]);

                Vector3 closestPoint = ClosestPointOnTriangle(worldPoint, a, b, c);

                float distance = Vector3.Distance(worldPoint, closestPoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }

            totalDistance += closestDistance;
        }

        return totalDistance / sourceVertices.Length;
    }

    /// <summary>
    /// Converts average geometric error into a 0-100 score.
    /// </summary>
    private float ErrorToScore(float error)
    {
        if (error <= perfectTolerance)
            return 100f;

        if (error >= failureDistance)
            return 0f;

        float normalized = Mathf.InverseLerp(failureDistance, perfectTolerance, error);

        return normalized * 100f;
    }

    /// <summary>
    /// Finds the closest point on triangle ABC to point P.
    ///
    /// Based on the standard closest-point-on-triangle
    /// region tests described by Ericson in
    /// Real-Time Collision Detection.
    /// </summary>
    private Vector3 ClosestPointOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ap = p - a;

        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);

        // Outside vertex A.
        if (d1 <= 0f && d2 <= 0f)
            return a;

        Vector3 bp = p - b;

        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);

        // Outside vertex B.
        if (d3 >= 0f && d4 <= d3)
            return b;

        float vc = d1 * d4 - d3 * d2;

        // On edge AB.
        if (vc <= 0f && d1 >= 0f && d3 <= 0f)
        {
            float v = d1 / (d1 - d3);

            return a + v * ab;
        }

        Vector3 cp = p - c;

        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);

        // Outside vertex C.
        if (d6 >= 0f && d5 <= d6)
            return c;

        float vb = d5 * d2 - d1 * d6;

        // On edge AC.
        if (vb <= 0f && d2 >= 0f && d6 <= 0f)
        {
            float w =
                d2 / (d2 - d6);

            return a + w * ac;
        }

        float va = d3 * d6 - d5 * d4;

        // On edge BC.
        if (va <= 0f && (d4 - d3) >= 0f && (d5 - d6) >= 0f)
        {
            float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));

            return b + w * (c - b);
        }

        // Inside the triangle.
        float denominator = 1f / (va + vb + vc);

        float vInside = vb * denominator;

        float wInside = vc * denominator;

        return a + ab * vInside + ac * wInside;
    }


    #endregion
}
