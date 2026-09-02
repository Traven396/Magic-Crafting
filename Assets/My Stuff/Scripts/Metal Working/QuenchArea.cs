using System.Collections.Generic;
using UnityEngine;

public class QuenchArea : MonoBehaviour
{
    [SerializeField] float CoolingSpeed = 10f;

    List<TemperatureController> CurrentInsertedObjects = new();


    private void FixedUpdate()
    {
        CoolObjects();
    }

    private void OnTriggerEnter(Collider other)
    {
        TemperatureController temperatureObject = other.GetComponent<TemperatureController>();

        if(!temperatureObject)
            temperatureObject = other.GetComponentInParent<TemperatureController>();

        if (temperatureObject && !CurrentInsertedObjects.Contains(temperatureObject))
        {
            CurrentInsertedObjects.Add(temperatureObject);
        }
    }

    void CoolObjects()
    {
        foreach (var heatedObject in CurrentInsertedObjects)
        {
            if (heatedObject.TemperatureInt > heatedObject.AmbientTemperature)
            {
                heatedObject.LowerTemperature(CoolingSpeed);
            }
        }
    }

    void FancyEffects(Vector3 position)
    {
        ShowParticles(position);
        PlayAudio(position);
    }

    void ShowParticles(Vector3 objectPosition)
    {

    }

    void PlayAudio(Vector3 objectPosition)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        TemperatureController temperatureObject = other.GetComponent<TemperatureController>();

        if(!temperatureObject)
            temperatureObject = other.GetComponentInParent<TemperatureController>();

        if (temperatureObject && CurrentInsertedObjects.Contains(temperatureObject))
        {
            CurrentInsertedObjects.Remove(temperatureObject);
        }
    }
}
