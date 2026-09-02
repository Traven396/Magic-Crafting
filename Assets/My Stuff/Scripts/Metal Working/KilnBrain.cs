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



    List<TemperatureController> currentHeatedObjects = new();

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
            foreach (var heatedObject in currentHeatedObjects)
            {
                if (heatedObject.TemperatureInt < MaxHeat)
                {
                    heatedObject.PausePassiveCooling = true;
                    heatedObject.RaiseTemperature(HeatingSpeed);
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
        TemperatureController heatedObject = other.GetComponentInParent<TemperatureController>();
        
        if (heatedObject && !currentHeatedObjects.Contains(heatedObject))
        {
            currentHeatedObjects.Add(heatedObject);
        }
    }

    public void OnTriggerExitCall(Collider other)
    {
        TemperatureController heatedObject = other.GetComponentInParent<TemperatureController>();

        if (heatedObject && currentHeatedObjects.Contains(heatedObject))
        {
            currentHeatedObjects.Remove(heatedObject);
            heatedObject.ResumePassiveCooling();
        }
    }

    public void OnTriggerStayCall(Collider other)
    {
        
    }
}
