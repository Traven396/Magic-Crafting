using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandControlArea : MonoBehaviour
{
    private List<Transform> insertedObjects = new List<Transform>();

    public bool ValidController = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!insertedObjects.Contains(other.transform))
                insertedObjects.Add(other.transform);

            if (insertedObjects.Count == 1)
                ValidController = true;
            else
                ValidController = false;
        }  
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (insertedObjects.Contains(other.transform))
                insertedObjects.Remove(other.transform);

            if (insertedObjects.Count == 1)
                ValidController = true;
            else
                ValidController = false;
        }
    }

    //Full 90 degrees to the left ends at 90 on the Z, neutral position is 360, full 90 degrees to the right ends at 270. 

    public float GetControllerValue()
    {
        if (ValidController)
        {
            var curRot = insertedObjects[0].parent.localRotation.eulerAngles.z;
            if(curRot < 180)
            {
                //This makes all numbers from 0 to 90 negative.
                //This is left rotations
                curRot *= -1;
            }else if (curRot > 180) 
            {
                //This makes all the right values go from 0 to 90.
                //This is for the right rotations
                curRot = 360 - curRot;
            }

            return curRot;
        }
        else
            return 0f;
    }
}
