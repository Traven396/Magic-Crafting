using Alchemy.Inspector;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class StarPatternPoint : MonoBehaviour
{

    [SerializeField] int Connections;

    StarGridManager gridManager;
    [SerializeField] Vector2 gridPos;
    [Title("Particles")]
    [SerializeField] GameObject CenterStarPS;
    [SerializeField] GameObject SwirlPS;

    public void Init(StarGridManager manager, Vector2 gridPosition)
    {
        gridManager = manager;
        gridPos = gridPosition;
    }

    public void AddConnection()
    {
        Connections++;

        if(Connections == 1)
        {
            //Turn on the center star visual
            CenterStarPS.SetActive(true);
            SwirlPS.SetActive(false);
        }

        if(Connections > 8)
        {
            Debug.Log("Something went wrong, connections should never be above 8");
        }
    }
    public int GetConnections()
    {
        return Connections;
    }

    public void RemoveConnection() 
    {
        Connections--;

        if(Connections == 0)
        {
            //Turn off the center star visual
            CenterStarPS.SetActive(false);
            SwirlPS.SetActive(true);
        }

        if(Connections < 0)
        {
            Debug.Log("Something went wrong, connections should never be negative");
        }
    }

    public void StarSelected(SelectEnterEventArgs args)
    {
        //Communicate to manager that this star was selected. Manager decides whether this is starting a connection, or finishing one
        gridManager.StarSelected(this, args.interactorObject as XRPokeInteractor);
    }

    public void StarHovered()
    {
        //Debug.Log("Star was hovered");
    }
}
