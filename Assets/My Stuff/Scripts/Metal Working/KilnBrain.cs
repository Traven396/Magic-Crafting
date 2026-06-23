using Alchemy.Inspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KilnBrain : MonoBehaviour, ITriggerable
{
    [Title("Heat Settings")]
    [SerializeField] int HeatingSpeed = 5;
    [SerializeField] int MaxHeat = 2200;
    
    [Title("Door Hinges")]
    [SerializeField] HingeJoint[] DoorJoints;



    List<KilnCrucible> currentInsertedCrucibles = new();

    float doorMoveTimer;

    [Button]
    void CloseDoors()
    {
        foreach (HingeJoint door in DoorJoints)
        {
            JointSpring spring = door.spring;

            spring.targetPosition = 0;
            spring.spring = 90;

            door.spring = spring;
        }
        doorMoveTimer = 0;
    }
    [Button]
    void OpenDoors()
    {
        foreach (HingeJoint door in DoorJoints)
        {
            JointSpring spring = door.spring;

            if (door.limits.max > Mathf.Abs(door.limits.min))
                spring.targetPosition = 90;
            else
                spring.targetPosition = -90;


            spring.spring = 90;

            door.spring = spring;
        }
        doorMoveTimer = 0;
    }

    private void FixedUpdate()
    {
        if (DoorJoints.All(joint => Mathf.Abs(joint.angle) < 4))
        {
            foreach (var crucible in currentInsertedCrucibles)
            {
                if (crucible.CurrentHeat < MaxHeat)
                {
                    crucible.IncreaseHeat(HeatingSpeed); 
                }
            }   
        }
    }

    private void Update()
    {
        if (doorMoveTimer != -69)
        {
            if (doorMoveTimer < 4.5)
            {
                doorMoveTimer += Time.deltaTime;
            }
            else
            {
                foreach (HingeJoint door in DoorJoints)
                {
                    JointSpring doorSpring = door.spring;

                    doorSpring.spring = 0;

                    door.spring = doorSpring;
                }

                doorMoveTimer = -69;
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
