using Alchemy.Inspector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.IO;
using System.Linq;
using UnityEngine.Events;

public class StarGridManager : MonoBehaviour
{
    [Header("Star Grid Settings")]
    [SerializeField] GameObject StarPrefab;
    [SerializeField] GameObject StarConnectionPrefab;
    [SerializeField] float StarSpacing;
    [SerializeField][Min(1)] int GridSize = 5;
    [SerializeField] float StarConnectionDistanceModifier = 1.5f;
    [Title("Pattern Settings")]
    public StarPattern[] PossiblePatterns;

    //The 1st dimension is the x, the 2nd is the y
    StarPatternPoint[,] StarGrid;

    List<Connection> Connections = new();
    //Need to spawn a grid of stars
    //Make it so I can control the size of the grid

    LineRenderer _lineRenderer;

    StarPatternPoint firstStarSelected = null;
    XRPokeInteractor currentInteractor;

    float validCheckTimerLength = 2;
    float validCheckTimer = -69;
    [Title("Events")]
    public UnityEvent OnStarSelected;
    public UnityEvent OnStarCancel;
    public UnityEvent OnConnectionMade;
    public UnityEvent<StarPattern> OnPatternSuccess;

    #region Debug
    [Title("Debug Settings")]
    public Vector2Int firstStarDebug;
    public Vector2Int secondStarDebug;


    [Button]
    void ManualCreateConnnection()
    {
        if (firstStarDebug == secondStarDebug)
        {
            firstStarDebug = secondStarDebug = Vector2Int.zero;
            return;
        }

        if (Math.Abs(firstStarDebug.x - secondStarDebug.x) > 1 || Math.Abs(firstStarDebug.y - secondStarDebug.y) > 1)
        {
            firstStarDebug = secondStarDebug = Vector2Int.zero;
            return;
        }

        var firstStar = StarGrid[firstStarDebug.x, firstStarDebug.y];
        var secondStar = StarGrid[secondStarDebug.x, secondStarDebug.y];

        Connection newConnection = new(firstStar, secondStar);

        if (Connections.Where(con => (con.start == newConnection.start && con.end == newConnection.end) || (con.start == newConnection.end && con.end == newConnection.start)).Any())
        {
            firstStarDebug = secondStarDebug = Vector2Int.zero;
            return;
        }

        //We would spawn a connection visual here. Dont worry now

        newConnection.start.AddConnection();
        newConnection.end.AddConnection();

        newConnection.visualLine = SpawnConnectionLine(firstStar.transform, secondStar.transform);

        Connections.Add(newConnection);

        firstStarDebug = secondStarDebug;
        secondStarDebug = Vector2Int.zero;
    }
    [Button]
    public void ClearGrid()
    {
        Connections.ForEach(con =>
        {
            con.start.RemoveConnection();
            con.end.RemoveConnection();
            Destroy(con.visualLine);
        });

        Connections.Clear();

        firstStarSelected = null;
    }
    [Button]
    void UndoLastConnection()
    {
        RemoveConnectionLine(Connections.Last());
    }
    [Button]
    void ForceCheckGrid() 
    {
        var validPattern = IsPatternValid();

        if (validPattern)
        {
            Debug.Log("Pattern is: " + validPattern.name);
            OnPatternSuccess?.Invoke(validPattern);
        }
        else
        {
            Debug.Log("Pattern is not valid");
        }
    }
    [Button]
    void WritePatternToFile()
    {
        if (!Application.isPlaying)
            return;


        string filePath = Path.Combine(Application.dataPath, "pattern.txt");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Connection Count: " + Connections.Count);

            for (int y = 0; y < GridSize; y++)
            {
                string line = y + ": ";
                for (int x = 0; x < GridSize; x++)
                {
                    line += StarGrid[x, y].GetConnections() + " ";
                }
                writer.WriteLine(line);
            }
        }
        Debug.Log($"Pattern written to {filePath}");
    } 
    #endregion

    private void Awake()
    {
        StarGrid = new StarPatternPoint[GridSize, GridSize];

        _lineRenderer = GetComponent<LineRenderer>();

        PossiblePatterns = Resources.LoadAll<StarPattern>("Star Pattern");

        CreateStarGrid();
    }

    private void Update()
    {
        UpdateLineRenderer();

        if(validCheckTimer > 0)
            validCheckTimer -= Time.deltaTime;
        else if (validCheckTimer != -69)
        {
            var validPattern = IsPatternValid();

            if (validPattern)
            {
                Debug.Log("Pattern is: " + validPattern.name);
                OnPatternSuccess?.Invoke(validPattern);
            }
            else
            {
                Debug.Log("Pattern is not valid");
            }

            validCheckTimer = -69;
        }
    }

    

    StarPattern IsPatternValid()
    {
        //First just check 
        foreach(StarPattern pattern in PossiblePatterns)
        {
            if (Connections.Count != pattern.ConnectionCount)
            {
                continue;
            }

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    if (StarGrid[x, y].GetConnections() != pattern.StarPatternGrid.GetCell(x, y))
                    {
                        goto invalidPattern;
                    }
                }
            }
            return pattern;

        invalidPattern:
            continue;
        }
        
        return null;
    }

    void CreateStarGrid()
    {
        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                GameObject newStar = Instantiate(StarPrefab, transform);

                newStar.transform.localPosition = new Vector3(x * StarSpacing, -y * StarSpacing, 0);
                StarGrid[x, y] = newStar.GetComponent<StarPatternPoint>();

                StarGrid[x, y].Init(this, new(x, y));
            }
        }
    }

    public void StarSelected(StarPatternPoint chosenStar, XRPokeInteractor interactor)
    {
        if (firstStarSelected) 
        {
            if (firstStarSelected == chosenStar)
            {
                //The player chose the same star twice. We ignore this
                return;
            }
            //This is where we finish a connection between stars and we create a full on connection between them.   

            Connection newConnection = new(firstStarSelected, chosenStar);

            if(Connections.Where(con => (con.start == newConnection.start && con.end == newConnection.end) || (con.start == newConnection.end && con.end == newConnection.start)).Any())
            {
                //The player is trying to connect two stars that are already connected. We ignore this
                firstStarSelected = null;

                return;
            }

            //We would spawn a connection visual here. Dont worry now

            newConnection.start.AddConnection();
            newConnection.end.AddConnection();

            newConnection.visualLine = SpawnConnectionLine(firstStarSelected.transform, chosenStar.transform);

            Connections.Add(newConnection);

            OnConnectionMade?.Invoke();

            firstStarSelected = chosenStar;

        } 
        else
        {
            firstStarSelected = chosenStar;
            currentInteractor = interactor;
            //We begin the connection process and start waiting for it to be finished
            //Maybe draw some kind of line between first star and the players finger.
        }


        OnStarSelected?.Invoke();
    }

    void RemoveConnectionLine(Connection connection)
    {
        if (Connections.Count == 0)
            return;

        var possibleCon = Connections.Where(c => c.visualLine == connection.visualLine);
        Connection conToRemove;

        if (possibleCon.Count() == 0)
            return;
        else
            conToRemove = possibleCon.FirstOrDefault();

        conToRemove.start.RemoveConnection();
        conToRemove.end.RemoveConnection();

        Destroy(conToRemove.visualLine);

        Connections.Remove(conToRemove);
    }
    public void RemoveConnectionLine(GameObject connectionLine)
    {
        if (Connections.Count == 0)
            return;

        var possibleCon = Connections.Where(c => c.visualLine == connectionLine);
        Connection conToRemove;

        if (possibleCon.Count() == 0)
            return;
        else
            conToRemove = possibleCon.FirstOrDefault();

        conToRemove.start.RemoveConnection();
        conToRemove.end.RemoveConnection();

        Destroy(conToRemove.visualLine);

        Connections.Remove(conToRemove);
    }

    GameObject SpawnConnectionLine(Transform startPoint, Transform endPoint)
    {
        GameObject currentLine = Instantiate(StarConnectionPrefab, transform);

        float lineLength = Vector3.Distance(startPoint.position, endPoint.position);

        currentLine.transform.localScale = new Vector3(1, 1, lineLength / 2f);

        //Calculate the middle between the two stars, and then place the line there
        Vector3 midPoint = (startPoint.position + endPoint.position) / 2;
        currentLine.transform.position = midPoint;

        //Then just make the line look at the end point, and make it rotate correctly
        currentLine.transform.LookAt(endPoint);

        validCheckTimer = validCheckTimerLength;

        currentLine.GetComponentInChildren<StarPatternConnection>().SpawnMethod(this);

        return currentLine;
    }

    void UpdateLineRenderer()
    {
        if (currentInteractor && firstStarSelected)
        {
            if(Vector3.Distance(currentInteractor.attachTransform.position, firstStarSelected.transform.position) > StarSpacing * StarConnectionDistanceModifier) 
            {

                currentInteractor = null;
                firstStarSelected = null;

                _lineRenderer.enabled = false;

                //Debug.Log("Canceled connection because player moved too far away from the starting star");

                OnStarCancel?.Invoke();

                return;
            }
            _lineRenderer.enabled = true;

            _lineRenderer.SetPosition(0, firstStarSelected.transform.position);
            _lineRenderer.SetPosition(1, currentInteractor.attachTransform.position);
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    struct Connection
    {
        public StarPatternPoint start;
        public GameObject visualLine;
        public StarPatternPoint end;

        public Connection(StarPatternPoint start, StarPatternPoint end) 
        {
            this.start = start;
            this.end = end;

            visualLine = null;
        }
    }
}
