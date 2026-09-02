using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellCircleTangle : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] float TimeScale = 1;
    [SerializeField] float TangleStreamEase = 0.2f;
    [SerializeField] int LinePointCount = 10;
    [SerializeField] float LineAmplitude = 1;
    [SerializeField] float LineFrequency = 2;
    [SerializeField] float LineScrollSpeed = 5;
    [SerializeField][Range(-1f, 1f)] float LinePull = 0;
    [SerializeField] bool DebugMode = false;
    

    [Header("References")]
    [SerializeField] Renderer _CircleRenderer;
    [SerializeField] ParticleSystem _Particles;
    [SerializeField] GameObject _CentralOrb;


    [SerializeField] GameObject _LinePrefab;
    [SerializeField] List<TangleStream> Tangles = new();

    LineRenderer testLine;
    float maxTimelineLength => 1 + TangleStreamEase * 2;

    float timeProgress;

    Transform[] debugSpheres;

    int customTimeID;

    bool displaying = false;
    bool hiding = false;

    [Button]
    void DisplayOverTime()
    {
        displaying = true;
        hiding = false;
    }
    [Button]
    void HideOverTime()
    {

        displaying = false;
        hiding = true;
    }

    void Start()
    {
        SetupLines();
        customTimeID = Shader.PropertyToID("_CustomTime");


        timeProgress = 0f;
        ProgressCircle();

        testLine = Instantiate(_LinePrefab, transform).GetComponent<LineRenderer>();
        testLine.positionCount = LinePointCount;

        debugSpheres = new Transform[LinePointCount];

        for (int i = 0; i < LinePointCount; i++)
        {
            var spawned = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spawned.transform.localScale = Vector3.one * 0.1f;
            spawned.name = i.ToString();

            spawned.transform.parent = transform;

            debugSpheres[i] = spawned.transform;

            if (!DebugMode)
            {
                spawned.SetActive(false);
            }
        }
    }

    void SetupLines()
    {
        foreach (var tangle in Tangles) 
        {
            var spawned = Instantiate(_LinePrefab, transform);
            tangle.Line = spawned.GetComponent<LineRenderer>();
            tangle.Line.positionCount = LinePointCount;
        }

        
    }
    private void Update()
    {
        if(DebugMode)
            SineWaveLine(testLine, transform.position + Vector3.up, transform.position + Vector3.up * 3, LinePull);


        if (displaying)
        {
            hiding = false;

            timeProgress = Mathf.MoveTowards(timeProgress, maxTimelineLength, TimeScale * Time.deltaTime);

            if (timeProgress >= maxTimelineLength)
                displaying = false;
        }

        if (hiding)
        {
            displaying = false;

            timeProgress = Mathf.MoveTowards(timeProgress, 0, TimeScale * Time.deltaTime);

            if (timeProgress <= 0)
                hiding = false;
        }

        ProgressCircle();

        foreach (var tangle in Tangles)
        {
            if (tangle.Line != null)
            {
                if (timeProgress == 0)
                {
                    if (tangle.Line.gameObject.activeInHierarchy)
                    {
                        tangle.Line.gameObject.SetActive(false);
                    }

                    continue;
                }

                

                if (timeProgress <= tangle.StartTime + TangleStreamEase && timeProgress >= tangle.StartTime)
                {
                    //This creates a small window where we will be moving the line from the center towards the outer edge

                    if (!tangle.Line.gameObject.activeInHierarchy)
                    {
                        tangle.Line.gameObject.SetActive(true);
                    }

                    //var lineProgress = (Progress - tangle.StartTime - 0.05f) / (tangle.StartTime - tangle.StartTime - 0.05f);
                    var lineProgress = (timeProgress - tangle.StartTime) / TangleStreamEase;


                    SineWaveLine(tangle.Line, transform.position, Vector3.Lerp(transform.position, tangle.EndPoint.position, lineProgress), 1-lineProgress);
                }
                else if (timeProgress > tangle.StartTime + TangleStreamEase && timeProgress < tangle.EndTime)
                {
                    //This is everything from its start time to a little before the end time

                    if (!tangle.Line.gameObject.activeInHierarchy)
                    {
                        tangle.Line.gameObject.SetActive(true);
                    }

                    SineWaveLine(tangle.Line, transform.position, tangle.EndPoint.position, 0);
                }
                else if (timeProgress < tangle.EndTime + TangleStreamEase && timeProgress >= tangle.EndTime)
                {
                    //Small window where we move the line from the center to the outer edge but from the back

                    if (!tangle.Line.gameObject.activeInHierarchy)
                    {
                        tangle.Line.gameObject.SetActive(true);
                    }

                    //var lineProgress = (Progress - tangle.EndTime - 0.15f) / (tangle.EndTime - tangle.EndTime - 0.15f);
                    var lineProgress = (timeProgress - tangle.EndTime) / TangleStreamEase;


                    SineWaveLine(tangle.Line, transform.position, tangle.EndPoint.position, lineProgress);
                }
                else
                {
                    if (tangle.Line.gameObject.activeInHierarchy)
                    {
                        tangle.Line.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    void ProgressCircle()
    {
        //We need to slightly reduce the value of CustomTime on material since the circle starts at 0 :(
        _CircleRenderer.material.SetFloat(customTimeID, Mathf.Clamp01(timeProgress - TangleStreamEase));

        _CentralOrb.transform.localScale = Vector3.one * (1 - timeProgress / (timeProgress + TangleStreamEase));

        
    }

    void SineWaveLine(LineRenderer lineRenderer, Vector3 startPoint, Vector3 endPoint, float pullAmount)
    {

        // Calculate positions relative to this object's pivot
        Vector3 toTarget = endPoint - startPoint;

        float totalLength = toTarget.magnitude;
        Vector3 direction = toTarget.normalized;

        // Define perpendicular vector (up axis) to handle the wave peaks correctly
        Vector3 upwardDir = Vector3.Cross(direction, Vector3.forward).normalized;
        if (upwardDir == Vector3.zero)
        {
            upwardDir = Vector3.up; // Fallback for pure Z-axis alignment
        }

        float currentPull = Mathf.Clamp(pullAmount, -1f, 1);

        for (int i = 0; i < LinePointCount; i++)
        {
            float progress = (float)i / (LinePointCount - 1);

            if (currentPull > 0)
            {
                if (progress < currentPull)
                {
                    progress = currentPull;

                    progress = Mathf.Clamp01(progress);
                }
            }
            else if (currentPull < 0)
            {
                if(progress > (1 + currentPull))
                {
                    progress = 1 + currentPull;

                    //progress = Mathf.Clamp01(progress);
                }
            }

            float distanceAlongLine = progress * totalLength;

            // Generate the raw scrolling sine wave
            float rawSine = Mathf.Sin((distanceAlongLine - Time.time * LineScrollSpeed) * LineFrequency);

            // Damping modifier: evaluates to 0 at progress=0, 1 at progress=0.5, and 0 at progress=1
            float endpointDamping = Mathf.Sin(((float)i / (LinePointCount - 1)) * Mathf.PI);

            // Combine them together with your amplitude
            float finalOffset = rawSine * endpointDamping * LineAmplitude;

            // Build the final position
            Vector3 worldPoint = startPoint + (direction * distanceAlongLine) + (upwardDir * finalOffset);

            if(DebugMode)
                debugSpheres[i].position = worldPoint;

            lineRenderer.SetPosition(i, worldPoint);
        }
    }


    [Serializable]
    class TangleStream
    {
        public Transform EndPoint;
        public LineRenderer Line { get; set; }
        [Range(0f, 1)] public float StartTime = 0f;
        [Range(0f, 1)] public float EndTime = 0f;
    }
}
