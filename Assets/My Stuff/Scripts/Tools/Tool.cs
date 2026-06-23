using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    //Tool will be the base class for all tools, all it does is add essentially an interface type functionality to this
    //I dont use an interface because I am not sure what all each tool will be able to do.
    //Every tool will implement UseTool in a different way, and call it at different times
    //The knife will call when the player scrapes it along the side of a correct ingredient.
    //
    //There will be another class for the ingredients that will have a method for being "processed" and will work by different things.
    public abstract void UseTool();
}
