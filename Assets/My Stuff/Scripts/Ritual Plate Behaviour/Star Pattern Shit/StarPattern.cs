using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "New Star Pattern", menuName = "Misc/Star Pattern")]
public class StarPattern : ScriptableObject
{
    //This will be some big array of stuff that we can read from to tell what a valid pattern is

    //Probably some kind of array showing their positions and what star needs how many connections
    [SerializeField]
    int connectionCount = 0;
    [SerializeField]
    private StarPattern2D starPattern = new(5);

    public StarPattern2D StarPatternGrid => starPattern;
    public int ConnectionCount => connectionCount;
}
