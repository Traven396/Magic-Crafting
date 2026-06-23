using Alchemy.Inspector;
using NUnit.Framework;
using System;
using UnityEngine;

public class CauldronStirStick : MonoBehaviour
{
    //IF this is negative we are CounterClock and positive its Clock
    //Ranges from -1 to 1
    public float CircleAmount;


    //An empty gameobject that exists somewhat in the center of the cauldron's surface.
    [SerializeField] Transform CauldronStirTransform;
    [Title("Stir Timing")]
    [SerializeField] float MinStirAnglePerStep = 10f;
    [SerializeField] int StepsPerSec = 10;
    [Title("Stirring Speeds")]
    [SerializeField] float MinimumStirSpeed = 1.5f;
    [SerializeField] float MaximumStirTime = 3;

    Rigidbody rb;

    float stepTime => 1 / (float)StepsPerSec;
    float timer = 0;


    public bool startCircling = false;

    Vector3 lastStepVector = Vector3.zero;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();

    }


    private void Update()
    {
        if (startCircling) 
        {
            timer += Time.deltaTime;

            Vector3 centerToStick = CauldronStirTransform.TransformDirection(CauldronStirTransform.position - (new Vector3(rb.position.x, CauldronStirTransform.position.y, rb.position.z)).normalized);


            if (timer >= stepTime)
            {
                if (lastStepVector != Vector3.zero)
                {
                    var angle = Vector3.SignedAngle(lastStepVector, centerToStick, CauldronStirTransform.up);

                    //If the angle is higher than the min required, then we are moving clockwise
                    if(angle >= MinStirAnglePerStep/* && rb.linearVelocity.magnitude > MinimumStirSpeed*/)
                    {
                        //Debug.Log("We are stirring clockwise");
                        CircleAmount = Mathf.MoveTowards(CircleAmount, 1.5f, 1 / (MaximumStirTime / stepTime));
                    } 
                    //If we are less than the negative amount, then we are moving counter-clockwise
                    else if (angle <= -MinStirAnglePerStep/* && rb.linearVelocity.magnitude > MinimumStirSpeed*/)
                    {
                        //Debug.Log("Counter=clockwise");
                        CircleAmount = Mathf.MoveTowards(CircleAmount, -1.5f, 1 / (MaximumStirTime / stepTime));
                    }
                    //If this wasnt true then we are not circling enough
                    else
                    {
                        CircleAmount = Mathf.MoveTowards(CircleAmount, 0, 1 / (MaximumStirTime * 2 / stepTime));
                    }
                }


                lastStepVector = centerToStick;
                timer = 0;
            }
        }
    }


}
