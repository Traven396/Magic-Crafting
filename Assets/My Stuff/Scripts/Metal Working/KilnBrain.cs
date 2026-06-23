using Alchemy.Inspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KilnBrain : MonoBehaviour, ITriggerable
{
    [Title("Heat Settings")]
    [SerializeField] int HeatingSpeed = 5;
    
    [Title("Door Hinges")]
    [SerializeField] HingeJoint[] DoorJoints;



    List<KilnCrucible> currentInsertedCrucibles = new();


    private void FixedUpdate()
    {
        if (DoorJoints.All(joint => Mathf.Abs(joint.angle) < 4))
        {
            foreach (var crucible in currentInsertedCrucibles)
            {
                crucible.IncreaseHeat(HeatingSpeed);
            } 
        }
    }



    public void OnTriggerEnterCall(Collider other)
    {
        KilnCrucible crucible = other.GetComponentInParent<KilnCrucible>();
        
        if (crucible)
        {
            if (!currentInsertedCrucibles.Contains(crucible))
            {
                currentInsertedCrucibles.Add(crucible);
            }
        }
    }

    public void OnTriggerExitCall(Collider other)
    {
        KilnCrucible crucible = other.GetComponentInParent<KilnCrucible>();

        if (crucible)
        {
            if(currentInsertedCrucibles.Contains(crucible))
            {
                currentInsertedCrucibles.Remove(crucible);
                
                crucible.BeginCooling();
            }
        }
    }

    public void OnTriggerStayCall(Collider other)
    {
        
    }
}
